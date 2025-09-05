pipeline {
  agent any

  options {
    timestamps()
  }

  environment {
    // Docker Hub kimlik bilgileri: Manage Jenkins > Credentials > (global) > ID: dockerhub-cred
    DOCKERHUB       = credentials('dockerhub-cred')
    DOCKER_NS       = 'sametbaglan'
    DOCKER_BUILDKIT = '1'
  }

  stages {

    stage('Checkout') {
      steps {
        checkout scm
        // Jenkins agent içindeki "dubious ownership" uyarısını sustur
        sh 'git config --global --add safe.directory "$PWD" || true'
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
          set -e
          # Çözüm dosyasını bul (ilk eşleşen)
          SLN="$(find . -maxdepth 3 -name "*.sln" | head -n1 || true)"
          echo "Solution: ${SLN}"

          docker run --rm -v "$PWD":/ws -w /ws mcr.microsoft.com/dotnet/sdk:8.0 bash -lc '
            set -e
            if [ -n "${SLN}" ] && [ -f "${SLN}" ]; then
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
          set -e
          TESTS="$(find . -name "*Test*.csproj" || true)"
          if [ -z "$TESTS" ]; then
            echo "No test projects found - continuing."
          else
            docker run --rm -v "$PWD":/ws -w /ws mcr.microsoft.com/dotnet/sdk:8.0 bash -lc '
              set -e
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
          // Dockerfile yolları: repo kökünden
          def services = [
            [name: 'basket',  path: 'EShopSln/Basket.Api'],
            [name: 'catalog', path: 'EShopSln/Catalog.Apii'],
            [name: 'order',   path: 'EShopSln/Order.Api'],
            [name: 'payment', path: 'EShopSln/Payment.Api'],
          ]

          services.each { svc ->
            sh """
              echo ">>> Building image for ${svc.name}"
              docker build \\
                -t ${DOCKER_NS}/${svc.name}:${VERSION} \\
                -f ${svc.path}/Dockerfile \\
                .   # <---- build context repo kökü
              docker tag ${DOCKER_NS}/${svc.name}:${VERSION} ${DOCKER_NS}/${svc.name}:latest
            """
          }
        }
      }
    }

    stage('Docker Push') {
      steps {
        sh 'echo "$DOCKERHUB_PSW" | docker login -u "$DOCKERHUB_USR" --password-stdin'
        script {
          def services = ['basket','catalog','order','payment']
          services.each { s ->
            sh """
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
          // compose dosyası kökte ya da EShopSln altında olabilir
          return fileExists('docker-compose.yml') || fileExists('compose.yaml') || fileExists('EShopSln/compose.yaml')
        }
      }
      steps {
        script {
          def composeFile = fileExists('docker-compose.yml') ? 'docker-compose.yml' :
                            (fileExists('compose.yaml') ? 'compose.yaml' :
                             (fileExists('EShopSln/compose.yaml') ? 'EShopSln/compose.yaml' : ''))
          sh """
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
