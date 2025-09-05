pipeline {
  agent any

  options {
    timestamps()
    //ansiColor('xterm')
  }

  environment {
    // Docker Hub kimlik bilgisi: Jenkins > Credentials'da oluşturdun (Username + Password)
    DOCKERHUB = credentials('dockerhub-cred')
    DOCKER_NS = 'sametbaglan'       // Docker Hub kullanıcı/organization adın
    DOCKER_BUILDKIT = '1'
    GIT_SHA = sh(script: "git rev-parse --short=8 HEAD || echo nosha", returnStdout: true).trim()
    VERSION = "${env.BUILD_NUMBER}-${GIT_SHA}"
  }

  triggers {
    // GitHub webhook bağlarsan otomatik tetikler
    githubPush()
  }

  stages {

    stage('Checkout') {
      steps { checkout scm }
    }

    stage('Dotnet Restore & Build') {
      steps {
        sh '''
          set -e
          # Çözüm dosyan büyük olasılıkla kökte: EShopSln.sln
          if [ -f ./EShopSln.sln ]; then
            dotnet restore ./EShopSln.sln
            dotnet build   ./EShopSln.sln -c Release --no-restore
          else
            # garanti olsun diye fallback
            find . -name "*.csproj" -print0 | xargs -0 -I{} dotnet restore "{}"
            find . -name "*.csproj" -print0 | xargs -0 -I{} dotnet build "{}" -c Release --no-restore
          fi
        '''
      }
    }

    stage('Tests') {
      steps {
        sh '''
          set -e
          # *Test*.csproj olanları çalıştır
          FOUND=0
          for p in $(find . -name "*Test*.csproj"); do
            FOUND=1
            dotnet test "$p" -c Release --no-build --logger trx || true
          done
          if [ "$FOUND" -eq 0 ]; then
            echo "No test projects found - continuing."
          fi
        '''
        junit allowEmptyResults: true, testResults: '**/TestResults/*.trx'
      }
    }

    stage('Docker Build') {
      steps {
        script {
          // Servis listesi: klasör ve dockerfile konumları ekran görüntündeki mimariye göre
          def services = [
            [name: 'basket',  path: 'src/Basket.Service/Presentation/Basket.Api'],
            [name: 'catalog', path: 'src/Catalog.Service/Presentation/Catalog.Api'],
            [name: 'order',   path: 'src/Order.Service/Presentation/Order.Api'],
            [name: 'payment', path: 'src/Payment.Service/Presentation/Payment.Api'],
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

    // (Opsiyonel) docker-compose varsa local/stage ayağa kaldır
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
  }

  post {
    success { echo "Build & Push OK -> ${DOCKER_NS}/*:${VERSION}" }
    always  { cleanWs() }
  }
}
