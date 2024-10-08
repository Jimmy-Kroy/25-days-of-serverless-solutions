# My notes about this solution:

The local.settings.json has the following attributes, which are read during startup in the programs.cs. The database configuration is then injected in the CosmosDbService object.
### local.settings.json file
```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
        "CosmosDB:EndpointUrl": "https://MyName.documents.azure.com:443/",
        "CosmosDB:PrimaryKey": "...",
        "CosmosDB:DatabaseName": "png-image-db",
        "CosmosDB:Container:Name": "png-image-container",
        "CosmosDB:Container:PartitionKey": "id"
    }
}
```

## References Used 🚀

-   **[Intro To CosmosDB](https://www.youtube.com/watch?v=ihFpgzDowcM)**
-   **[Multiple Azure Functions in the Same File](https://dontpaniclabs.com/blog/post/2023/03/24/multiple-azure-functions-in-the-same-file/)**
-   **[Best Design Pattern for Azure Cosmos DB Containers — Factory Pattern](https://medium.com/swlh/best-design-pattern-for-azure-cosmos-db-containers-factory-pattern-addff5628f8a)**
-   **[Azure Cosmos DB with ASP.NET Core Web API](https://code-maze.com/azure-cosmos-db-with-asp-net-core-web-api/)**
-   **[Migrate local.settings.json settings to Azure](https://learn.microsoft.com/en-us/azure/azure-functions/functions-develop-vs/)**

<hr>



# Challenge 3: Webhooks

![Webhooks](https://res.cloudinary.com/jen-looper/image/upload/v1575132446/images/challenge-3_zj98pl.jpg)

## Secret Santa's Furry Friends

'Tis the season for gift giving! Here at Microsoft HQ in Redmond, we're excited for our annual Secret Santa gift swap! Each employee who chooses to participate is assigned another coworker to give a gift to. Rather than put a price limit on gifts, though, Satya has decided that this year everyone is just going to send their favorite cute animal picture. To make sure people can't easily figure out who their Secret Santa is, he wants to make sure that all of the photos are stored in the same format (`png`) and are made available from a single database.

For this challenge, create a web service that gets called everytime a commit or push is made to a Github repository. If the commit has a file ending with `.png`, your service should take the URL to the image from Github and store it in whatever database you like.

## Resources/Tools Used 🚀

-   **[Visual Studio](https://visualstudio.microsoft.com?WT.mc_id=25daysofserverless-github-cxa)**
-   **[Postman](https://www.getpostman.com/downloads/)**
-   **[Get started with webhooks using Github](https://codeburst.io/whats-a-webhook-1827b07a3ffa)**
-   **[Webhook tester](http://webhook.site/)**

## Next Steps 🏃

Learn more about serverless with a Free Training!

-   ✅ **[Serverless Free Courses](https://docs.microsoft.com/learn/browse/?term=azure%20functions&WT.mc_id=25daysofserverless-github-cxa)**
## Important Resources ⭐️

Here include all the important features related to the challenges that are integrated into microsoft.docs. Ex.:

-   ✅ **[Azure Functions documentation](https://docs.microsoft.com/azure/azure-functions/?WT.mc_id=25daysofserverless-github-cxa)**
-   ✅ **[Create your first Function app using Visual Studio](https://docs.microsoft.com/azure/azure-functions/functions-develop-vs?WT.mc_id=25daysofserverless-github-cxa)**
-   ✅ **[Free E-Book - Azure Serverless Computing Cookbook, Second Edition](https://azure.microsoft.com/resources/azure-serverless-computing-cookbook/?WT.mc_id=25daysofserverless-github-cxa)**

## Questions? Comments? ❓

If you have any doubts about the challenges, feel free to open an **[ISSUE HERE](https://github.com/microsoft/25-days-of-serverless/issues)**. As soon as possible we will be answering any questions/doubts that you may have!
