pipeline {
	agent any
    options {
        // This is required if you want to clean before build
        skipDefaultCheckout(true)
    }
	stages {
		stage('Checkout') {
			steps {
                cleanWs()
				checkout scm
			}
		}

		stage('Additional Fetch/Checkout') {
			when {
				tag 'v*'
			}
			steps {
				// This fetch and checkout sequence is a workaround for the quirky way Jenkins pipeline builds handle git checkouts
				// It is required for gitversion to work properly
				bat 'git fetch origin main:main'
				bat 'git checkout %BRANCH_NAME%'
			}
		}

		stage('Versioning') {
			when {
				anyOf {
					branch 'main'
					tag 'v*'
				}
			}
			steps {
				bat 'git fetch --tags'
				bat 'dotnet-gitversion /output buildserver'

				script {
					def props = readProperties file: 'gitversion.properties'

					env.GitVersion_SemVer = props.GitVersion_SemVer
					env.GitVersion_MajorMinorPatch = props.GitVersion_MajorMinorPatch
				}
			}
		}

		stage('Database') {
			steps {
				bat 'nuke RestoreTestDatabase'
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
				anyOf {
					branch 'main'
					tag 'v*'
				}
			}
			steps {
				bat 'nuke Publish'
				bat 'dotnet octo pack --basePath="./publish/bin" --id="HeliosDiscordBot" --version="%GitVersion_SemVer%" --format=zip'
				bat 'curl -X POST %OCTOPUS_ENDPOINT% -H "X-Octopus-ApiKey: %OCTOPUS_API_KEY%" -F "data=@HeliosDiscordBot.%GitVersion_SemVer%.zip"'
				bat 'dotnet octo pack --basePath="./publish/db" --id="HeliosDiscordBot-db" --version="%GitVersion_SemVer%" --format=zip'
				bat 'curl -X POST %OCTOPUS_ENDPOINT% -H "X-Octopus-ApiKey: %OCTOPUS_API_KEY%" -F "data=@HeliosDiscordBot-db.%GitVersion_SemVer%.zip"'
				bat 'dotnet octo create-release --project HeliosDiscordBot --version %GitVersion_SemVer% --server %OCTOPUS_ENDPOINT% --apiKey %OCTOPUS_API_KEY%'
				bat 'dotnet octo deploy-release --project HeliosDiscordBot --releaseNumber %GitVersion_SemVer% --deployto Stage --server %OCTOPUS_ENDPOINT% --apiKey %OCTOPUS_API_KEY%'
			}
		}
	}
}
