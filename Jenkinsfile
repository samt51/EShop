pipeline {
  agent any

  options {
    timestamps()
    timeout(time: 45, unit: 'MINUTES')
    disableResume()
  }

  environment {
    // dockerhub-cred = Username + Password/Token (UsernamePassword credentials)
    DOCKERHUB       = credentials('dockerhub-cred')
    DOCKER_BUILDKIT = '1'
  }

  stages {
    stage('Checkout') {
      steps {
        checkout scm
        sh 'git config --global --add safe.directory "$PWD" || true'
      }
    }

    stage('Version Compute') {
      steps {
        script {
          env.GIT_SHA   = sh(returnStdout: true, script: 'git rev-parse --short=8 HEAD || echo nosha').trim()
          env.VERSION   = "${env.BUILD_NUMBER}-${env.GIT_SHA}"
          env.DOCKER_NS = sh(returnStdout: true, script: 'printf "%s" "$DOCKERHUB_USR"').trim()
          echo "VERSION=${env.VERSION}  DOCKER_NS=${env.DOCKER_NS}"
        }
      }
    }

    stage('Dotnet Restore & Build') {
      steps {
        timeout(time: 15, unit: 'MINUTES') {
          sh '''
            set -eu
            SLN="$(find . -maxdepth 3 -name "*.sln" | head -n1 || true)"
            echo "Solution: ${SLN}"

            docker run --rm -v "$PWD":/ws -w /ws -e SLN="$SLN" mcr.microsoft.com/dotnet/sdk:8.0 bash -lc '
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
    }

    stage('Tests') {
      steps {
        timeout(time: 10, unit: 'MINUTES') {
          sh '''
            set -eu
            TESTS="$(find . -name "*Test*.csproj" || true)"
            if [ -z "$TESTS" ]; then
              echo "No test projects found - continuing."
            else
              docker run --rm -v "$PWD":/ws -w /ws mcr.microsoft.com/dotnet/sdk:8.0 bash -lc '
                set -euo pipefail
                for p in $(find . -name "*Test*.csproj"); do
                  echo "Running tests in $p"
                  dotnet test "$p" -c Release --no-build --logger trx || true
                done
              '
            fi
          '''
        }
        junit allowEmptyResults: true, testResults: '**/TestResults/*.trx'
      }
    }

    stage('Docker Login') {
      steps {
        retry(2) {
          sh 'set -eu; echo "$DOCKERHUB_PSW" | docker login -u "$DOCKERHUB_USR" --password-stdin'
        }
      }
    }

    stage('Docker Build & Push') {
      steps {
        script {
          def services = [
            [name: 'basket',  path: 'EShopSln/Basket.Api'],
            [name: 'catalog', path: 'EShopSln/Catalog.Apii'],
            [name: 'order',   path: 'EShopSln/Order.Api'],
            [name: 'payment', path: 'EShopSln/Payment.Api'],
          ]

          services.each { svc ->
            stage("Build ${svc.name}") {
              timeout(time: 20, unit: 'MINUTES') {
                sh """
                  set -eu
                  echo ">>> Building image for ${svc.name}"
                  docker build \\
                    --build-arg BUILD_VERSION=${VERSION} \\
                    --build-arg GIT_SHA=${GIT_SHA} \\
                    -t ${DOCKER_NS}/${svc.name}:${VERSION} \\
                    -f ${svc.path}/Dockerfile \\
                    .
                  docker tag ${DOCKER_NS}/${svc.name}:${VERSION} ${DOCKER_NS}/${svc.name}:latest
                """
              }
            }
            stage("Push ${svc.name}") {
              timeout(time: 10, unit: 'MINUTES') {
                retry(2) {
                  sh """
                    set -eu
                    echo ">>> Pushing ${svc.name}"
                    docker push ${DOCKER_NS}/${svc.name}:${VERSION}
                    docker push ${DOCKER_NS}/${svc.name}:latest
                  """
                }
              }
            }
          }
        }
      }
      post {
        always {
          sh 'docker logout || true'
          sh 'docker builder prune -af || true'
        }
      }
    }
stage('Deploy (docker-compose)') {
  timeout(time: 10, unit: 'MINUTES') {
    script {
      def compose = 'EShopSln/compose.yaml'
      echo "Using compose file: ${compose}"

      // 1) Eski projeleri temizle (varsa) – hata verse bile job düşmesin
      sh """
        set -eu
        DOCKER_NS=${DOCKER_NS} VERSION=${VERSION} docker compose -f ${compose} down -v --remove-orphans || true
        docker container prune -f || true
        docker image prune -f || true
      """

      // 2) Yeni imajları çek
      sh "DOCKER_NS=${DOCKER_NS} VERSION=${VERSION} docker compose -f ${compose} pull"

      // 3) Tertemiz şekilde ayağa kaldır
      sh "DOCKER_NS=${DOCKER_NS} VERSION=${VERSION} docker compose -f ${compose} up -d --force-recreate"
    }
  }
}

  post {
    success { echo "Build & Push OK -> ${DOCKER_NS}/*:${VERSION}" }
    always  { cleanWs() }
  }
}
