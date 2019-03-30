pipeline {
    agent any
    stages {
        stage('clear-nugets') {
            steps {               
                echo "Clearing nuget packages"
                sh "dotnet nuget locals --clear all"                       
            }
        }
        stage('restore-nugets') {
            steps {
                echo "Restoring project geonames.sln"
                sh "dotnet restore geonames.sln" 
            }
        }
        stage('build-application') {
            steps {
                echo "Building project geonames.sln"
                sh "dotnet build geonames.sln"              
            }
        }
        stage('publish-application') {
            steps {
                echo "Publishing project geonames.sln"
                sh "dotnet publish geonames.sln --configuration Release"
            }
        }
        // Package up output in dist folder as a nuget 
        stage('package-application') {
            steps {
                // Move to the publishPhysicalFolder folder and create the nupkg file in the workspace folder using zip.
                echo "Package up output in dist folder as a nuget"
//                sh "cd ${publishFolder} && zip -r ${workspaceFolder}/${nupkgFileName} . && cd ${workspaceFolder}"
            }
        }
        // Push nuget build to artifactory, make new octopus build pointing to that
        // stage('deploy-application') {
        //     steps {
        //         echo "Publish nuget build to Artifactory"
        //         sh "dotnet nuget push ${nupkgFileName} --api-key ${globals.artifactoryUserName}:${globals.artifactoryNugetPassword} --source https://${artifactoryPublishUrl}"

        //         echo "Make new octopus build pointing to that"
        //         // Create the JSON file and save it in the current folder.
        //         createOctopusRelease(
        //             projectId: nupkgOctopusProjectId, 
        //             version: fullVersionNumber, 
        //             stepName: nupkgOctopusStepName, 
        //             actionName: nupkgOctopusActionName, 
        //             octopusChannelId: octopusChannelId,
        //             buildUrl: buildUrl
        //         )
        //     }
        // }
    }
}
