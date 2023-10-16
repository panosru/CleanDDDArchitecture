// Define the pipeline
pipeline {
  // This pipeline can be run on any Jenkins agent
  agent any
  
  // Define environment variables
  environment {
    // Define the home directory for SonarScanner
    MSBuildScannerHome = tool 'SonarScanner for MSBuild v5'
  }
  
  // Define the stages of the pipeline
  stages {
    
    // Stage for beginning SonarQube analysis
    stage('Begin SonarQube Analysis') {
      steps {
        // Skip this build if the commit message contains [CI-SKIP]
        scmSkip(deleteBuild: true, skipPattern:'.*\\[CI-SKIP\\].*')
        script {
          // Set SonarQube environment variables
          withSonarQubeEnv() {
            // Begin SonarQube analysis
            sh "dotnet ${MSBuildScannerHome}/SonarScanner.MSBuild.dll begin /k:\"panosru_CleanDDDArchitecture\""
          }
        }
      }
    }
    
    // Stage for building the project
    stage('Building') {
      steps {
        // Skip this build if the commit message contains [CI-SKIP]
        scmSkip(deleteBuild: true, skipPattern:'.*\\[CI-SKIP\\].*')
        // Restore dependencies and tools for the project
        sh 'dotnet restore Aviant.sln'
        // Build the project
        sh "dotnet build"
      }
    }
    
    // Stage for completing SonarQube analysis
    stage('Complete SonarQube Analysis') {
      steps {
        // Skip this build if the commit message contains [CI-SKIP]
        scmSkip(deleteBuild: true, skipPattern:'.*\\[CI-SKIP\\].*')
        script {
          // Set SonarQube environment variables
          withSonarQubeEnv() {
            // End SonarQube analysis
            sh "dotnet ${MSBuildScannerHome}/SonarScanner.MSBuild.dll end"
          }
        }
      }
    }
    
    // Stage for checking the SonarQube Quality Gate status
    stage('Quality Gate') {
      steps {
        // Skip this build if the commit message contains [CI-SKIP]
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
