# Web Reverse Gate
A simple "reverse proxy" application that can record and manipulate traffic passing through it. For each application it publishes, it provides a webhook address that can be triggered through "git" service providers and execute predefined command strings in sequence. It can rival applications like IIS and Nginx 

![flow_image](https://raw.githubusercontent.com/meto260/WebReverseGate/master/flow.jpg)

<hr />
<span style="color:red;">A portable timeseries database Questdb is required!</span>

[Download Questdb](https://questdb.io/get-questdb/)


<hr />

<code>appsettings.json</code>
```Json
"ReverseProxy": {
    "Routes": {
      "route1": {
        "ClusterId": "cluster1",
        "Match": {
          "Path": "{**catch-all}"
        }
      }
    },
    "Clusters": {
      "cluster1": {
        "Destinations": {
          "destination1": {
            "Address": "https://example.com"
          }
        }
      }
    }
  }
```
<hr />

<code>apps.yml</code>

```Yaml
containers:
    - apps:
        appname: testname
        runbefore:
            - taskkill /f /IM back4mvc.exe
            - cd C:\temp\apptest1
            - git clone https://github.com/meto260/back4mvc.git
            - cd C:\temp\apptest1\back4mvc
            - git pull
        runafter:
            - taskkill /f /IM back4mvc.exe
            - cd C:\temp\apptest1\back4mvc
            - dotnet build
            - cd back4mvc
            - dotnet run
```
<hr />

<code>Example github webhook trigger</code>

![](https://raw.githubusercontent.com/meto260/WebReverseGate/master/github_example_webhook_config.png)
