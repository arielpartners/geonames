@Library('ariel-utils') _

// nodeLabel controls which Jenkins slaves this project can build on, and is required.
//String nodeLabel = required value: parms.get('nodeLabel'), message: "'nodeLabel' parameter not set in the Jenkinsfile.  This parameter is required."

// versionFile specifies a file that contains the version number that will be used for the entire project package, and is required.
String versionFile //= required value: parms.get('versionFile'), message: "'versionFile' parameter not set in the Jenkinsfile.  This parameter is required."

// projectName specifies a string that uniquely identifies the project, and is used to build several output filenames.  This is required.
String projectName = 'geonames'

// projectFile specifies a project file to be passed as a parameter to the build command, and is required when the main project folder contains more than one project or solution file.
String projectFile = 'geonames.webapi/geonames.webapi.csproj'

// artifactoryPublishRepo specifies the sub-folder under 'builds' within Artifactory to deploy the build package to, and is required.
// This path will be created in Atrifactory by Jenkins.
//String artifactoryPublishRepo = required value: parms.get('artifactoryPublishRepo'), message: "Parameter 'artifactoryPublishRepo' was not defined - this is required to indicate the publish destination in Artifactory."
//String artifactoryPublishUrl = "${globals.artifactoryRoot}/${globals.artifactoryServerName}/api/${artifactoryPublishRepo}/"

// artifactoryNugetRepo specifies the folder within Artifactory to source Nuget packages from.
//String artifactoryNugetRepo = parms.get("artifactoryNugetRepo", globals.artifactoryNugetRepo)
//String artifactoryNugetUrl = "${globals.artifactoryRoot}/${globals.artifactoryServerName}/api/${artifactoryNugetRepo}"

// init Phase 1 (always executes): - Setup variables and print out values for debugging
String publishFolder = "publish"
String publishPhysicalFolder

String nupkgFileName
String nuspecFileName
String nugetConfig
String nuspecId = projectName
String nuspecAuthor = "Ken Granderson"
String nuspecDescription = "Placeholder for Nuspec Description"

String branchName
String buildNumber

String version
String versionSuffix

String nodeName

String workspaceFolder

String jobName
String buildUrl
String buildUrlSlackMessage
String buildUrlHtmlMessage
//String octopusChannelId

String dateBuildNumber
String fullVersionNumber = ""
String notificationHtmlFooter = ''
String notificationSlackFooter = ''

/*
String nupkgOctopusProjectId = parms.get("nupkgOctopusProjectId", "")
String nupkgOctopusStepName = parms.get("nupkgOctopusStepName", "")
String nupkgOctopusActionName = parms.get("nupkgOctopusActionName", "")
String nupkgOctopusUnstableChannelId = parms.get("nupkgOctopusUnstableChannelId", "")
String nupkgOctopusDefaultChannelId = parms.get("nupkgOctopusDefaultChannelId", "Default")

String artifactoryZipOctopusProjectId = parms.get("artifactoryZipOctopusProjectId", "")
String artifactoryZipOctopusStepName = parms.get("artifactoryZipOctopusStepName", "")
String artifactoryZipOctopusActionName = parms.get("artifactoryZipOctopusActionName", "")
*/

/*
Boolean deployNuget = parms.get("deployNuget", false)

String slackChannel = parms.slackChannel
String slackWebHook = parms.get("slackWebHook", "${globals.dhsSlackWebHookUrl}")

Boolean isPR = isPullRequest()
Boolean isRel = isRelease()
*/

pipeline {
    agent any
    stages {
        stage('initialization') {
            steps {
                echo "init - Setup variables and print out values for debugging"
                script {
                    Boolean abortedMasterBuild = false

                    try {
                        // Get environment values set by Jenkins.
                        branchName = "${env.BRANCH_NAME}"
                        buildNumber = "${env.BUILD_NUMBER}"
                        jobName = "${env.JOB_NAME}"
                        nodeName = "${env.NODE_NAME}"
                        workspaceFolder = "${env.WORKSPACE}"
                        buildUrl = "${env.RUN_DISPLAY_URL}"
//                        buildUrlSlackMessage = "Click <${buildUrl}|here> to view job details"
//                        buildUrlHtmlMessage = "<p>Click <a href=\'${buildUrl}\'>here</a> to view job details.</p>"

/*
                        // Validate branch name.
                        Boolean branchNameValid = validateBranchName(branchName)

                        if (!branchNameValid) {
                            abortedMasterBuild = true
                            def errorMessage = "CAF CI/CD Compliance Error: Branch name '${branchName}' does not conform to GitFlow conventions - aborting..."
                            echo errorMessage
                            error(errorMessage)
                        }
*/
                        // Read the project version from the version file.
                        version = '1.0.0' //readVersion versionFile
                       
                        // Generate unique date / build stamp for file names
                        dateBuildNumber = buildSerialNumber buildNumber

                        // Generate the full version number.
//                        fullVersionNumber = generateVersionNumber version, branchName, dateBuildNumber
    
    // Feature branch builds, hotfixes and pull requests (which are not deployed)
    // have a string suffix appended to the version number.
    // Release builds do not, as they presume that the developer has
    // manually updated the version with 'rc#' if necessary.
    /*String*/ versionSuffix = 
        branchName =~ /^(feature\/|develop)/ ? '-unstable' : 
        branchName =~ /^hotfix\// ? '-hotfix': 
        branchName =~ /^PR-\d+$/ ? '-pr' :
        ''

    // If the build is getting a string suffix, then the unique 
    // build serial number is added to it.
    if (versionSuffix != '') {
        versionSuffix += ".${dateBuildNumber}"
    }

    /*String*/ fullVersionNumber = "${version}${versionSuffix}"

                        // Extract the version suffix.
                        versionSuffix = fullVersionNumber.substring(version.length());


/*                        
                        nugetConfig = "${workspaceFolder}/nuget.config"

                        // If projectFile is specified (not using default blank if there is only one target in the folder)
                        // verify that it exists.
                        if (projectFile != '') {       
                            Boolean exists = fileExists "${workspaceFolder}/${projectFile}"

                            if (!exists) {
                                error "The project file to build '${projectFile} does not exist!'"
                            }
                        }
*/
                        // Define publish folder path.
                        publishPhysicalFolder = "${workspaceFolder}/${publishFolder}"

                        // Delete existing publish folder.
                        echo "Deleting any existing publish folder ${publishPhysicalFolder}"
                        sh "rm -rf ${publishPhysicalFolder}"

                        nupkgFileName = "${projectName}.${fullVersionNumber}.nupkg"
                        nuspecFileName = "${projectName}.${fullVersionNumber}.nuspec"

                        // get the channelID
//                        octopusChannelId = branchName =~ /^(feature\/|develop)/ ? "${nupkgOctopusUnstableChannelId}": "${nupkgOctopusDefaultChannelId}"

                        // Configure build
//                        buildDotnetCommand += " /p:NoPackageAnalysis=true"


                        // Send build start notification.
//                        sendBuildStartNotification notified, jobName, buildNumber, nodeName, buildUrl
                        }
                    catch (Exception e) {
                        // Check for abort due to master build attempt.
                        if (abortedMasterBuild) {
                            currentBuild.result = 'ABORTED'
                            throw e
                            // return here instead of throwing error to keep the build "green"
                            // return
                        }

                        echo e.toString()
                    }
                }
            } // /steps
        } // /stage
        stage('show-job-parameters') {
            steps {               
                echo """
branchName: $branchName
buildNumber: $buildNumber
dateBuildNumber: $dateBuildNumber
fullVersionNumber: $fullVersionNumber
gitChecksum: ${env.GIT_COMMIT}
jobName: $jobName
nodeName: $nodeName
nupkgFileName: $nupkgFileName
projectName: $projectName
publishFolder: $publishFolder
version: $version
versionFile: $versionFile
versionSuffix: $versionSuffix
workspaceFolder: $workspaceFolder
"""
            }
        }
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
                sh "cd publish && zip -r ${workspaceFolder}/${nupkgFileName} . && cd ${workspaceFolder}"
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
