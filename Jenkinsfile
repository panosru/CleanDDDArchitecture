pipeline {
  agent any
  
  environment {
    MSBuildScannerHome = tool 'SonarScanner for MSBuild v5'
  }
  
  stages {
    
    stage('Begin SonarQube Analysis') {
      steps {
        scmSkip(deleteBuild: true, skipPattern:'.*\\[CI-SKIP\\].*')
        script {
          withSonarQubeEnv() {
            sh "dotnet ${MSBuildScannerHome}/SonarScanner.MSBuild.dll begin /k:\"panosru_CleanDDDArchitecture\""
          }
        }
      }
    }
    
    stage('Building') {
      steps {
        scmSkip(deleteBuild: true, skipPattern:'.*\\[CI-SKIP\\].*')
        sh 'dotnet restore Aviant.DDD.sln'
        sh "dotnet build"
      }
    }
    
    stage('Complete SonarQube Analysis') {
      steps {
        scmSkip(deleteBuild: true, skipPattern:'.*\\[CI-SKIP\\].*')
        script {
          withSonarQubeEnv() {
            sh "dotnet ${MSBuildScannerHome}/SonarScanner.MSBuild.dll end"
          }
        }
      }
    }
    
    stage('Quality Gate') {
      steps {
        scmSkip(deleteBuild: true, skipPattern:'.*\\[CI-SKIP\\].*')
        echo 'Waiting for quality gate to pass'
        timeout(time: 2, unit: 'MINUTES') {
          waitForQualityGate abortPipeline: true
        }
      }
    }
  }
  
  post {
    cleanup {
      catchError(buildResult: null, stageResult: 'FAILURE', message: 'Cleanup Failure') {
        echo 'Cleaning up workspace...'
        cleanWs()
      }
    }
  }
}
