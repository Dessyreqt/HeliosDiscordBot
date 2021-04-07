pipeline {
	agent any
	stages {
		stage('Checkout') {
			steps {
				checkout scm
			}
		}

		stage('Database') {
			when {
				branch 'main'
			}
			steps {
				bat 'nuke RestoreDatabase'
			}
		}

		stage('Build') {
			steps {
				bat 'nuke Clean'
				bat 'nuke Restore'
				bat 'nuke Compile'
			}
		}

		stage('Deploy') {
			when {
				branch 'main'
			}
			steps {
				powershell 'Stop-Service HeliosDiscordBot'
				bat 'nuke Publish'
				powershell 'Start-Service HeliosDiscordBot'
			}
		}
	}
}
