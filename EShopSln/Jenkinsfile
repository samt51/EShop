pipeline {
  agent any

  options { timestamps() }

  environment {
    DOCKERHUB       = credentials('dockerhub-cred')
    DOCKERHUB_USR   = "${DOCKERHUB_USR}"
    DOCKERHUB_PSW   = "${DOCKERHUB_PSW}"
    DOCKER_BUILDKIT = '1'
  }

  stages {

    stage('Checkout') {
      steps {
        checkout scm
        sh 'git config --global --add safe.directory "$PWD" || true'
      }
    }

    stage('Init Env') {
      steps {
        script {
          env.DOCKER_NS = env.DOCKERHUB_USR
          echo "Docker namespace set to: ${env.DOCKER_NS}"
        }
      }
    }

    stage('Version Compute') {
      steps {
        script {
          env.GIT_SHA = sh(returnStdout: true, script: 'git rev-parse --short=8 HEAD || echo nosha').trim()
          env.VERSION = "${env.BUILD_NUMBER}-${env.GIT_SHA}"
          echo "VERSION=${env.VERSION}"
        }
      }
    }

    stage('Dotnet Restore & Build') {
      steps {
        sh '''
          set -eu
          SLN="$(find . -maxdepth 3 -name "*.sln" | head -n1 || true)"
          echo "Solution: ${SLN}"

          docker run --rm -v "$PWD":/ws -w /ws -e SLN="$SLN" mcr.microsoft.com/dotnet/sdk:8.0 bash -lc '
            set -eu
            if [ -n "${SLN:-}" ] && [ -f "${SLN:-}" ]; then
              dotnet restore "${SLN}"
              dotnet build   "${SLN}" -c Release --no-restore
            else
              find . -name "*.csproj" -print0 | xargs -0 -I{} dotnet restore "{}"
              find . -name "*.csproj" -print0 | xargs -0 -I{} dotnet build "{}" -c Release --no-restore
            fi
          '
        '''
      }
    }

    stage('Tests') {
      steps {
        sh '''
          set -eu
          TESTS="$(find . -name "*Test*.csproj" || true)"
          if [ -z "$TESTS" ]; then
            echo "No test projects found - continuing."
          else
            docker run --rm -v "$PWD":/ws -w /ws mcr.microsoft.com/dotnet/sdk:8.0 bash -lc '
              set -eu
              for p in $(find . -name "*Test*.csproj"); do
                echo "Running tests in $p"
                dotnet test "$p" -c Release --no-build --logger trx || true
              done
            '
          fi
        '''
        junit allowEmptyResults: true, testResults: '**/TestResults/*.trx'
      }
    }

    stage('Docker Build') {
      steps {
        script {
          def services = [
            [name: 'basket',  path: 'EShopSln/Basket.Api'],
            [name: 'catalog', path: 'EShopSln/Catalog.Apii'], // klasör ismi gerçekten 'Catalog.Apii' ise bırak
            [name: 'order',   path: 'EShopSln/Order.Api'],
            [name: 'payment', path: 'EShopSln/Payment.Api'],
          ]

          services.each { svc ->
            sh """
              set -eu
              echo ">>> Building image for ${svc.name}"
              docker build \
                -t ${DOCKER_NS}/${svc.name}:${VERSION} \
                -f ${svc.path}/Dockerfile \
                .
              docker tag ${DOCKER_NS}/${svc.name}:${VERSION} ${DOCKER_NS}/${svc.name}:latest
            """
          }
        }
      }
    }

    stage('Docker Push') {
      steps {
        sh 'set -eu; echo "$DOCKERHUB_PSW" | docker login -u "$DOCKERHUB_USR" --password-stdin'
        script {
          def services = ['basket','catalog','order','payment']
          services.each { s ->
            sh """
              set -eu
              echo ">>> Pushing ${s}"
              docker push ${DOCKER_NS}/${s}:${VERSION}
              docker push ${DOCKER_NS}/${s}:latest
            """
          }
        }
      }
    }

    stage('Deploy (docker-compose)') {
      when {
        expression {
          fileExists('docker-compose.yml') || fileExists('compose.yaml') || fileExists('EShopSln/compose.yaml')
        }
      }
      steps {
        script {
          def composeFile = fileExists('docker-compose.yml') ? 'docker-compose.yml' :
                            (fileExists('compose.yaml') ? 'compose.yaml' :
                             (fileExists('EShopSln/compose.yaml') ? 'EShopSln/compose.yaml' : ''))
          sh """
            set -eu
            echo "Using compose file: ${composeFile}"
            docker compose -f ${composeFile} pull
            docker compose -f ${composeFile} up -d --remove-orphans
            docker compose -f ${composeFile} ps
          """
        }
      }
    }
  }

  post {
    success {
      echo "Build & Push OK -> ${DOCKER_NS}/*:${VERSION}"
    }
    always {
      cleanWs()
    }
  }
}
