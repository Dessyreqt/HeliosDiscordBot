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
				tag 'v*'
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

		
		stage('Publish') {
			when {
				branch 'main'
			}
			steps {
				bat 'nuke Publish'
				bat 'dotnet octo pack --basePath="./publish/bin" --id="HeliosDiscordBot" --version="0.0.1-%BUILD_NUMBER%" --format=zip'
				bat 'curl -X POST %OCTOPUS_ENDPOINT% -H "X-Octopus-ApiKey: %OCTOPUS_API_KEY%" -F "data=@HeliosDiscordBot.0.0.1-%BUILD_NUMBER%.zip"'
				bat 'dotnet octo pack --basePath="./publish/db" --id="HeliosDiscordBot-db" --version="0.0.1-%BUILD_NUMBER%" --format=zip'
				bat 'curl -X POST %OCTOPUS_ENDPOINT% -H "X-Octopus-ApiKey: %OCTOPUS_API_KEY%" -F "data=@HeliosDiscordBot-db.0.0.1-%BUILD_NUMBER%.zip"'
				bat 'dotnet octo create-release --project HeliosDiscordBot --version 0.0.1-%BUILD_NUMBER% --server %OCTOPUS_ENDPOINT% --apiKey %OCTOPUS_API_KEY%'
				bat 'dotnet octo deploy-release --project HeliosDiscordBot --releaseNumber 0.0.1-%BUILD_NUMBER% --deployto Stage --server %OCTOPUS_ENDPOINT% --apiKey %OCTOPUS_API_KEY%'
			}
		}
	}
}
