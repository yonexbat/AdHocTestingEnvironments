# AdHocTestingEnvironments
## Einleitung
*AdHocTesting Environments* ist eine Lösung, um bei Bedarf eine Testumgebung zu starten oder zu stoppen. Die Lösung basiert auf *Kubernetes* und *.net Core*. In einem Webinterface kann der User die zu startende Applikation wählen und die Anzahl Stunden definieren, wo die Applkation aktiv sein soll. Mit Click auf Starten wird dann die Applikation aufgestartet. Semesterarbeit *CAS Cloud Computing* der *Berner Fachhochschule* von Claude Glauser und Pascal Hermann.

## Wissenswertes
- Für die Interaktion mit Kubernetes API gibt es zwei Implementationen. Einmal eine, wo die Kubernetes API direkt angesprochen wird. Die andere Implementation funktiert zusammen mit *git* und *ArgoCD*.
- Neue Applikationen können über eine API registriert werden.
- Der Zugang zu den zu testenden Applikationen geht über einen YARP (Yet Another Reverse Proxy). https://microsoft.github.io/reverse-proxy/

## Status
Implementiert als *Proof of Concept*.
## Konfiguration
Die Konfiguration befindet sich in der Datei *appsettings.json*.
| Key    | Value       |
| ------ | ----------- |
| KubernetesAccessToken | Access Token für die Kubernetes API. Nur relevant, wenn nicht git Variante verwendet wird. |
| KubernetesHost | Server URL, wo die Kubernetes API läuft. Beispiel: https://FFFFFF98B54E8F7CA989F8DC512B881.gr7.eu-central-1.eks.amazonaws.com |
| KubernetesNamespace | Kubernetes Namespace |
| UseGitClient | true wenn *git* und *ArgoCD* verwendet werden soll.|
| GitUrl | Url zum Git Repository. https://github.com/abc/def.git |
| GitUser | User für commits |
| GitPw |  Passwort für commits |
| GitBranch | Branch zum hinpushen der Änderungen. Bsp: *main* |

## Applikationen
Es wurden zwei Beispielapplikationen erstellt. Eine .net Core Webapplikation und eine Angular.io Applikation:
- https://github.com/yonexbat/SampleWebapp DockerHub: https://hub.docker.com/repository/docker/claudeglauser/sample-webapp
- https://github.com/yonexbat/sampleangularapp DockerHub: https://hub.docker.com/repository/docker/claudeglauser/sampleangularapp

