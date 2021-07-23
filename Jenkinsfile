#!groovy
@Library('pipeline-library@master')_


pipeline {
    agent { label 'linux-slaves && dotnet' }
    options {
        buildDiscarder(logRotator(numToKeepStr:'10'))
        ansiColor('xterm')
        timestamps()
    }
    environment{
        projectVersion = getDotnetVersion(versionField: 'VersionPrefix')
        projectName = "adhoctestingenvironments"
    }
    tools {
        jdk 'jdk-8u131'
    }    
    stages {
        stage('Prepare') {
          steps {
            cleanWs()
            checkout scm
          }
        }
        stage('Set dotnet sdk') {
            steps {
                setDotnetSdk(
                    version: '5.0'
                )
            }
        }
        stage('Build porject') {
            steps {
                dotnetBuild(
                    project: './AdHocTestingEnvironments.sln'
                )
            }
        }
        stage('Publish .net') {
            steps {
                dotnetPublish(projectPath:'AdHocTestingEnvironments')
            }
        }        
        stage('Docker build') {
             steps{
                dockerBuild(
                    projectName: "${projectName}",
                    tag: projectVersion,
                    buildOptions: ['pull','no-cache','rm=true'],
                    path: './AdHocTestingEnvironments'
                )
            }
        }
        stage('Docker push') {
            steps{
                dockerPush(
                    projectName: "${projectName}",
                    tag: projectVersion,
                    credentialsId: "05e790a3-2dfd-4a30-b846-513dfcfa152f"
                )
            }
        }
        /*
        stage('Set dotnet sdk') {
            steps {
                setDotnetSdk(
                    version: '3.1'
                )
            }
        }
        stage('Build') {
            steps {
                dotnetBuild(
                    project: './'+solutionName+'.sln'
                )
            }
        }
         stage('Test') {
            steps {
                dotnetTest(
                    project: './tests/'+solutionName+'.Tests.Component/'+solutionName+'.Tests.Component.csproj',
                    category: 'Unit'
                )                
                dotnetTest(
                    project: './tests/'+solutionName+'.Tests.Component/'+solutionName+'.Tests.Component.csproj',
                    category: 'Integration'
                )
                sh("echo success")
            }
        }
        stage('Publish') {
            steps {
                dotnetPublish(projectPath:'src/Dfb.Backend.Service')
            }
        }
        stage('Docker build') {
             steps{
                dockerBuild(
                    projectName: "app/dfb/${projectName}",
                    tag: projectVersion,
                    buildOptions: ['pull','no-cache','rm=true'],
                    path: './src/'+solutionName+'.Service'
                )
            }
        }
        stage('Docker push') {
            when{
                anyOf {
                    branch 'master'
                    branch 'integration'
                }
            }
            steps{
                dockerPush(
                    projectName: "app/dfb/${projectName}",
                    tag: projectVersion,
                    credentialsId: "05e790a3-2dfd-4a30-b846-513dfcfa152f"
                )
            }
        }
        stage('update Kustomize') {
            steps {
                gitOpsUpdate(
                    "bitbucketCredential":"s-cicd-dotnet",
                    "projectVersion":projectVersion
                )
            }
        }
        stage('Integration'){
            when {
                branch 'integration'
            }
            stages{
                stage('INT - Create GitOpsPR') {
                    steps {
                        gitOpsPR(
                            "bitbucketProject":"DFB",
                            "bitbucketRepository":"dfb-int",
                            "bitbucketCredential":"s-cicd-dotnet",
                            "projectVersion":projectVersion,
                            "automerge":"true"
                        )
                    }
                }
                stage('INT - wait for deployment'){
                    steps{
                        argoCdWaitForDeployment(
                            "server":"https://deployment-eks-int-m02cn0002.eks.aws.pnetcloud.ch",
                            "application":"dfb-int",
                            "serviceName":"backend-service",
                            "projectVersion": projectVersion
                        )                                                                        
                    }
                }
                stage('Integration tests')
                {
                    steps{
                        sh("echo success")
                    }
                }
            }
        }
        stage('Production'){
            when {
                branch 'master'
            }
            stages{
                stage('PROD - Create GitOpsPR') {
                    steps {
                        gitOpsPR(
                            "bitbucketProject":"dfb",
                            "bitbucketRepository":"dfb-prod",
                            "bitbucketCredential":"s-cicd-dotnet",
                            "projectVersion":projectVersion,
                            "automerge":"true"
                        )
                    }
                }
                stage('PROD - wait for deployment'){
                    steps{
                        argoCdWaitForDeployment(
                            "server":"https://deployment-eks-prod-m02cp0002.eks.aws.pnetcloud.ch",
                            "application":"dfb-prod",
                            "serviceName":"backend-service",
                            "projectVersion": projectVersion
                        )
                    }
                }
            }
        }*/
    }
}
