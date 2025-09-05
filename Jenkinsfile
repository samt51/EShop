pipeline {
  agent any

  options { timestamps() }

  environment {
    DOCKERHUB = credentials('dockerhub-cred')   // Jenkins > Credentials ID
    DOCKER_NS = 'sametbaglan'                   // Docker Hub kullanıcı/organization adın
    DOCKER_BUILDKIT = '1'
    GIT_SHA  = sh(script: "git rev-parse --short=8 HEAD || echo nosha", returnStdout: true).trim()
    VERSION  = "${env.BUILD_NUMBER}-${GIT_SHA}"
  }

  triggers { githubPush() }

  stages {

    stage('Checkout') {
      steps { checkout scm }
    }

    stage('Dotnet Restore & Build') {
      steps {
        sh '''
          set -e
          SLN="$(find . -maxdepth 3 -name '*.sln' | head -n1 || true)"
          echo "Solution: ${SLN:-none}"

          docker run --rm -v "$PWD":/ws -w /ws mcr.microsoft.com/dotnet/sdk:8.0 bash -lc "
            set -e
            if [ -n \\"$SLN\\" ] && [ -f \\"$SLN\\" ]; then
              dotnet restore \\"$SLN\\"
              dotnet build   \\"$SLN\\" -c Release --no-restore
            else
              find . -name '*.csproj' -print0 | xargs -0 -I{} dotnet restore '{}'
              find . -name '*.csproj' -print0 | xargs -0 -I{} dotnet build   '{}' -c Release --no-restore
            fi
          "
        '''
      }
    }

    stage('Tests') {
      steps {
        sh '''
          set -e
          docker run --rm -v "$PWD":/ws -w /ws mcr.microsoft.com/dotnet/sdk:8.0 bash -lc "
            set -e
            FOUND=0
            for p in \$(find . -name '*Test*.csproj'); do
              FOUND=1
              echo Running tests in \$p
              dotnet test \\"$p\\" -c Release --no-build --logger trx || true
            done
            if [ \\"$FOUND\\" -eq 0 ]; then
              echo 'No test projects found - continuing.'
            fi
          "
        '''
        junit allowEmptyResults: true, testResults: '**/TestResults/*.trx'
      }
    }

    stage('Docker Build') {
      steps {
        script {
          def services = [
            [name: 'basket',  path: 'EShopSln/src/Basket.Service/Presentation/Basket.Api'],
            [name: 'catalog', path: 'EShopSln/src/Catalog.Service/Presentation/Catalog.Api'],
            [name: 'order',   path: 'EShopSln/src/Order.Service/Presentation/Order.Api'],
            [name: 'payment', path: 'EShopSln/src/Payment.Service/Presentation/Payment.Api'],
          ]
          for (svc in services) {
            sh """
              echo ">>> Building image for ${svc.name}"
              docker build -t ${DOCKER_NS}/${svc.name}:${VERSION} -f ${svc.path}/Dockerfile ${svc.path}
              docker tag  ${DOCKER_NS}/${svc.name}:${VERSION} ${DOCKER_NS}/${svc.name}:latest
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
          for (svc in services) {
            sh """
              echo ">>> Pushing ${svc}"
              docker push ${DOCKER_NS}/${svc}:${VERSION}
              docker push ${DOCKER_NS}/${svc}:latest
            """
          }
        }
      }
    }

    stage('Deploy (docker-compose)') {
      when { expression { fileExists('docker-compose.yml') } }
      steps {
        sh '''
          docker compose pull
          docker compose up -d --remove-orphans
          docker compose ps
        '''
      }
    }

    stage('Cleanup') {
      steps { deleteDir() }
    }
  }
}
