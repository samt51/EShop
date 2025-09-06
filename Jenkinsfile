pipeline {
  agent any

  environment {
    DOCKERHUB_CRED = credentials('dockerhub-cred')   // Jenkins Credentials ID
    GITHUB_PAT     = credentials('github-pat')       // (SCM erişimi için)
    DOCKER_NS      = "samt51"
    DOCKER_BUILDKIT = "1"
  }

  options {
    timestamps()
    buildDiscarder(logRotator(numToKeepStr: '20'))
    timeout(time: 45, unit: 'MINUTES')
  }

  stages {
    stage('Checkout') {
      steps {
        checkout scm
        sh '''
          set -e
          git config --global --add safe.directory "$WORKSPACE"
        '''
      }
    }

    stage('Version Compute') {
      steps {
        script {
          def shortSha = sh(returnStdout: true, script: 'git rev-parse --short=8 HEAD').trim()
          env.VERSION = "7-${shortSha}"
          echo "VERSION=${env.VERSION}  DOCKER_NS=${env.DOCKER_NS}"
        }
      }
    }

    stage('Dotnet Restore & Build') {
      options { timeout(time: 15, unit: 'MINUTES') }
      steps {
        sh '''
          set -eu
          SLN=$(find . -maxdepth 3 -name "*.sln" | head -n1)
          echo "Solution: ${SLN}"
          docker run --rm -e DOTNET_CLI_TELEMETRY_OPTOUT=1 \
            -v "$PWD":/ws -w /ws -e SLN="${SLN}" mcr.microsoft.com/dotnet/sdk:8.0 bash -lc '
            set -euo pipefail
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
      options { timeout(time: 10, unit: 'MINUTES') }
      steps {
        sh '''
          set -eu
          TESTS=$(find . -name "*Test*.csproj" || true)
          if [ -z "${TESTS}" ]; then
            echo "No test projects found - continuing."
          else
            docker run --rm -e DOTNET_CLI_TELEMETRY_OPTOUT=1 \
              -v "$PWD":/ws -w /ws mcr.microsoft.com/dotnet/sdk:8.0 bash -lc '
              set -euo pipefail
              for p in $(find . -name "*Test*.csproj"); do
                dotnet test "$p" -c Release --no-build --logger "trx;LogFileName=$(basename "$p").trx"
              done
            '
          fi
        '''
        junit allowEmptyResults: true, testResults: '**/*.trx'
      }
    }

    stage('Docker Login') {
      steps {
        retry(2) {
          withCredentials([usernamePassword(
            credentialsId: 'dockerhub-cred',
            usernameVariable: 'DOCKERHUB_USR',
            passwordVariable: 'DOCKERHUB_PSW'
          )]) {
            sh '''
              set -eu
              echo "$DOCKERHUB_PSW" | docker login -u "$DOCKERHUB_USR" --password-stdin
              docker info >/dev/null 2>&1
            '''
          }
        }
      }
    }

    stage('Docker Build & Push') {
      stages {
        stage('Build basket') {
          options { timeout(time: 20, unit: 'MINUTES') }
          steps {
            sh '''
              set -eu
              echo ">>> Building image for basket"
              docker build --pull --build-arg BUILD_VERSION="${VERSION}" --build-arg GIT_SHA="$(git rev-parse --short=8 HEAD)" \
                -t "${DOCKER_NS}/basket:${VERSION}" -f EShopSln/Basket.Api/Dockerfile .
              docker tag "${DOCKER_NS}/basket:${VERSION}" "${DOCKER_NS}/basket:latest"
            '''
          }
        }
        stage('Push basket') {
          options { timeout(time: 10, unit: 'MINUTES') }
          steps {
            retry(2) {
              sh '''
                set -eu
                echo ">>> Pushing basket"
                docker push "${DOCKER_NS}/basket:${VERSION}"
                docker push "${DOCKER_NS}/basket:latest"
              '''
            }
          }
        }

        stage('Build catalog') {
          options { timeout(time: 20, unit: 'MINUTES') }
          steps {
            sh '''
              set -eu
              echo ">>> Building image for catalog (Catalog.Apii)"
              docker build --pull --build-arg BUILD_VERSION="${VERSION}" --build-arg GIT_SHA="$(git rev-parse --short=8 HEAD)" \
                -t "${DOCKER_NS}/catalog:${VERSION}" -f EShopSln/Catalog.Apii/Dockerfile .
              docker tag "${DOCKER_NS}/catalog:${VERSION}" "${DOCKER_NS}/catalog:latest"
            '''
          }
        }
        stage('Push catalog') {
          options { timeout(time: 10, unit: 'MINUTES') }
          steps {
            retry(2) {
              sh '''
                set -eu
                echo ">>> Pushing catalog"
                docker push "${DOCKER_NS}/catalog:${VERSION}"
                docker push "${DOCKER_NS}/catalog:latest"
              '''
            }
          }
        }

        stage('Build order') {
          options { timeout(time: 20, unit: 'MINUTES') }
          steps {
            sh '''
              set -eu
              echo ">>> Building image for order"
              docker build --pull --build-arg BUILD_VERSION="${VERSION}" --build-arg GIT_SHA="$(git rev-parse --short=8 HEAD)" \
                -t "${DOCKER_NS}/order:${VERSION}" -f EShopSln/Order.Api/Dockerfile .
              docker tag "${DOCKER_NS}/order:${VERSION}" "${DOCKER_NS}/order:latest"
            '''
          }
        }
        stage('Push order') {
          options { timeout(time: 10, unit: 'MINUTES') }
          steps {
            retry(2) {
              sh '''
                set -eu
                echo ">>> Pushing order"
                docker push "${DOCKER_NS}/order:${VERSION}"
                docker push "${DOCKER_NS}/order:latest"
              '''
            }
          }
        }

        stage('Build payment') {
          options { timeout(time: 20, unit: 'MINUTES') }
          steps {
            sh '''
              set -eu
              echo ">>> Building image for payment"
              docker build --pull --build-arg BUILD_VERSION="${VERSION}" --build-arg GIT_SHA="$(git rev-parse --short=8 HEAD)" \
                -t "${DOCKER_NS}/payment:${VERSION}" -f EShopSln/Payment.Api/Dockerfile .
              docker tag "${DOCKER_NS}/payment:${VERSION}" "${DOCKER_NS}/payment:latest"
            '''
          }
        }
        stage('Push payment') {
          options { timeout(time: 10, unit: 'MINUTES') }
          steps {
            retry(2) {
              sh '''
                set -eu
                echo ">>> Pushing payment"
                docker push "${DOCKER_NS}/payment:${VERSION}"
                docker push "${DOCKER_NS}/payment:latest"
              '''
            }
          }
        }
      }
    }

    stage('Deploy (docker-compose)') {
      when { branch 'main' }   // sadece main deploy olsun; istersen kaldır
      options { timeout(time: 10, unit: 'MINUTES') }
      steps {
        script {
          def composeFile = 'EShopSln/compose.yaml'
          if (!fileExists(composeFile)) {
            error "Compose file not found: ${composeFile}"
          }
        }
        sh '''
          set -eu
          echo "Using compose file: EShopSln/compose.yaml"
          export COMPOSE_CMD='docker compose -p eshopci -f EShopSln/compose.yaml'

          DOCKER_NS="${DOCKER_NS}" VERSION="${VERSION}" $COMPOSE_CMD pull
          DOCKER_NS="${DOCKER_NS}" VERSION="${VERSION}" $COMPOSE_CMD down --remove-orphans || true
          DOCKER_NS="${DOCKER_NS}" VERSION="${VERSION}" $COMPOSE_CMD up -d --force-recreate
          DOCKER_NS="${DOCKER_NS}" VERSION="${VERSION}" $COMPOSE_CMD ps
        '''
      }
    }

    stage('Cleanup') {
      when { expression { true } }
      steps {
        sh '''
          set +e
          docker logout || true
          docker builder prune -af || true
        '''
        cleanWs(deleteDirs: true, notFailBuild: true)
      }
    }
  }

  post {
    failure {
      // Deploy aşaması patlarsa logları topla (özellikle port çakışmaları için faydalı)
      script {
        sh '''
          set +e
          export COMPOSE_CMD='docker compose -p eshopci -f EShopSln/compose.yaml'
          $COMPOSE_CMD ps || true
          $COMPOSE_CMD logs --no-color --tail=300 || true
        '''
      }
    }
    always { echo 'Pipeline finished.' }
  }
}
