# Solution
I created the FindImageApp using an Azure HTTP triggered function. The function is called by passing query parameters in the GET request.
## The end point: [GET] /api/FindImageApp
```
GET /api/FindImage?query=cars&mode=url
GET /api/FindImage?query=yellow%20car&mode=url
```
The query parameter defines the search criteria for the image search. The mode parameter determines the return format, which can be "url", "file", or "redirect".

* If mode is set to "file", the first image found is downloaded to the local computer.
* When mode is set to "redirect", the browser is redirected to the link of the first image found.
* When mode is set to "url", a JSON file containing links to the first three image results is returned, as shown in the Json file below. 

```json
[
  {
    "id": "dKUGgIi7C-c",
    "regular_url": "https://images.unsplash.com/photo-1530550912241-cd4506bfc27d?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w2MzYwOTF8MHwxfHNlYXJjaHwxfHx5ZWxsb3clMjBjYXJ8ZW58MHx8fHwxNzIyMTY3Nzg1fDA&ixlib=rb-4.0.3&q=80&w=1080",
    "download_url": "https://unsplash.com/photos/dKUGgIi7C-c/download?ixid=M3w2MzYwOTF8MHwxfHNlYXJjaHwxfHx5ZWxsb3clMjBjYXJ8ZW58MHx8fHwxNzIyMTY3Nzg1fDA",
    "alt_description": "yellow panel van parked near green leafed trees",
    "FileName": "yellow_panel_van_parked_near_green_leafed_trees.png"
  },
  {
    "id": "XR8kg_TYGh4",
    "regular_url": "https://images.unsplash.com/photo-1523998172836-07d4ac80b873?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w2MzYwOTF8MHwxfHNlYXJjaHwyfHx5ZWxsb3clMjBjYXJ8ZW58MHx8fHwxNzIyMTY3Nzg1fDA&ixlib=rb-4.0.3&q=80&w=1080",
    "download_url": "https://unsplash.com/photos/XR8kg_TYGh4/download?ixid=M3w2MzYwOTF8MHwxfHNlYXJjaHwyfHx5ZWxsb3clMjBjYXJ8ZW58MHx8fHwxNzIyMTY3Nzg1fDA",
    "alt_description": "Ford Mustang on road surrounded by trees",
    "FileName": "Ford_Mustang_on_road_surrounded_by_trees.png"
  },
  {
    "id": "hKkqBI9JPZE",
    "regular_url": "https://images.unsplash.com/photo-1532876443100-8b02d4490723?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w2MzYwOTF8MHwxfHNlYXJjaHwzfHx5ZWxsb3clMjBjYXJ8ZW58MHx8fHwxNzIyMTY3Nzg1fDA&ixlib=rb-4.0.3&q=80&w=1080",
    "download_url": "https://unsplash.com/photos/hKkqBI9JPZE/download?ixid=M3w2MzYwOTF8MHwxfHNlYXJjaHwzfHx5ZWxsb3clMjBjYXJ8ZW58MHx8fHwxNzIyMTY3Nzg1fDA",
    "alt_description": "closed yellow car",
    "FileName": "closed_yellow_car.png"
  }
]
``` 
## Azure Key fault
The Unsplash Api key is stored in Azure Key fault. The FindImageApp function uses a system assigned identity to access the fault and retrieve the Api key. 

* In order to add and remove secrets from the Key Fault, you need to assign the "Key Vault Administrator" role to your account, as can be read **[here](https://learn.microsoft.com/en-us/answers/questions/1370440/azure-keyvault-the-operation-is-not-allowed-by-rba)**.
* If your key vault is configured as "Azure role-based access control", then assign Key Vault Secrets User role to the application.
* If your key vault is configured as "Vault access policy", then you have to create access policy selecting Secret permissions and assigning it to application. Read more **[here](https://stackoverflow.com/questions/77760514/how-to-retrieve-secret-from-azure-key-vault-from-console-application)**.

The local.settings.json looks like this:
```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
        "UnsplashClient:EndpointUrl": "https://api.unsplash.com/",
        "UnsplashClient:KeyVaultName": "unsplash-api-keys",
        "UnsplashClient:SecretName": "Access-Key"
    }
}
``` 

## Resources/Tools Used

-   **[Unsplash API Documentation](https://unsplash.com/documentation#search-photos)**
-   **[KeyVaultCache class](https://mcguirev10.com/2017/12/23/easy-configuration-sharing-with-azure-key-vault.html)**
-   **[Azure Function: Get secret from Azure Key Vault](https://community.dynamics.com/blogs/post/?postid=f85ae982-2756-4750-8385-bd386dec5c3e)**
-   **[How to retrieve Azure Key Vault Secrets using Azure Functions](https://turbo360.com/blog/how-to-retrieve-azure-key-vault-secrets-using-azure-functions-part-i)**
-   **[Accessing Azure Key Vault Secrets with Azure Functions](https://medium.com/@dssc2022yt/accessing-azure-key-vault-secrets-with-azure-functions-2e651980f292)**
-   **[Use Azure Key Vault](https://www.loginradius.com/blog/engineering/guest-post/using-azure-key-vault-with-an-azure-web-app-in-c-sharp/)**
-   **[Connect an Azure function with an Azure key vault](https://medium.com/@ssbmqtjt/how-to-connect-an-azure-function-with-an-azure-key-vault-azure-portal-and-python-bd5140178a7)**
-   **[Connect To Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/general/tutorial-net-create-vault-azure-web-app?tabs=azure-cli)**
-   **[How to use managed identities](https://learn.microsoft.com/en-us/azure/app-service/overview-managed-identity?tabs=portal%2Chttp)**

# Challenge 7: API Endpoint - Picture Challenge

![A Virtual Bonfire](https://res.cloudinary.com/jen-looper/image/upload/v1575132446/images/challenge-7_kzcrtm.jpg)

December 7 marks the first day of the official Christmas season in Guatemala. Everybody is scrambling to get ready for the big _la quema del diablo_ (burning of the devil) tonight — at 6pm sharp, everyone will start a bonfire to burn rubbish and items they don't need to cleanse their homes of evil.

Here in Guatemala City, our friend Miguel is concerned about the environmental impact! The past few years, people have been burning a lot of rubber and plastic that makes the air dirty. Some places are switching to burning paper piñatas of the devil, but Miguel still wants to let people metaphorically cleanse their houses of specific items they don't want.

Let's help Miguel by building a web API that lets his neighbors search for images of things they want to get rid of. Build an application (e.g. a cloud function with a single endpoint) that takes text as an input and returns an image found on unsplash or another image platform.

## Resources/Tools Used 🚀
Here are the tools listed that we used for an example solution.

-   **[Azure Functions with Java](https://docs.microsoft.com/azure/azure-functions/functions-create-first-java-maven/?WT.mc_id=25daysofserverless-github-sakriema)**
-   **[Java JSON parser org.json](https://search.maven.org/classic/#search%7Cgav%7C1%7Cg%3A%22org.json%22%20AND%20a%3A%22json%22)**
-   **[UNSPLASH Picture API](https://unsplash.com/)**
-   **[Postman](https://www.getpostman.com/downloads/)**


## Tips 🔥

Make sure to keep your keys private. Profit e.g. from environment variables to do so;

```bash
> export UNSPLASH_ACCESS_KEY="your_access_key"
> export UNSPLASH_SECRET_KEY="your_secret_key"
```

## Other Resources ⭐️

Other helpful Resources can be found here:

-   ✅ **[Azure Functions documentation](https://docs.microsoft.com/azure/azure-functions?WT.mc_id=25daysofserverless-github-sakriema)**
-   ✅ **[Visual Studio Code](https://code.visualstudio.com/?WT.mc_id=25daysofserverless-github-sakriema)**
-   ✅ **[Create your first function using Visual Studio Code](https://docs.microsoft.com/azure/azure-functions/functions-create-first-function-vs-code?WT.mc_id=25daysofserverless-github-sakriema)**
-   ✅ **[Azure Functions Extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions&WT.mc_id=25daysofserverless-github-sakriema)**
-   ✅ **[Free E-Book - Azure Serverless Computing Cookbook, Second Edition](https://azure.microsoft.com/resources/azure-serverless-computing-cookbook/?WT.mc_id=25daysofserverless-github-sakriema)**


## I have doubts ... What do I do?! ❓

If you have any doubts about the challenges, feel free to open an **[ISSUE HERE](https://github.com/microsoft/25-days-of-serverless/issues)**. As soon as possible we will be answering any questions/doubts that you may have!
