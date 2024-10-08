# Solution 

I developed the DealSeekerApp using an Azure timer-triggered function. This function runs every hour, as specified by the TimerTrigger parameter set to _("0 0 * * * *")_. The purpose of this time-triggered function is to search for great deals on X. To save costs (since X requires a paid API subscription), I’m utilizing a mock object to simulate returning great deals.    

In addition, I created an HTTP-triggered function called _GetDealsOfTheDay_. This function retrieves all the deals of the day, as shown below.

## The end point: [GET] /api/GetDealsOfTheDay
```
[GET] /api/GetDealsOfTheDay
```
The Json response of the _GetDealsOfTheDay_ is shown below.
```json
[
  {
    "description": "Quiet Hybrid Spectrum 20W Bug Zapper Fruit Fly Traps for Indoors Mosquito Zapper Electric Fly Zapper for Home Mosquito Repellent Moth Light Gnat Insect Killer with 2 Replacement Bulbs",
    "price": 39.98,
    "link": "https://www.amazon.com/Spectrum-Mosquito-Electric-Repellent-Replacement/dp/B0BVKGC941"
  },
  {
    "description": "SAMSUNG Galaxy Tab S6 Lite (2024) 10.4\" 64GB WiFi Android Tablet, S Pen Included, Gaming Ready, Long Battery Life, Slim Metal Design, Expandable Storage, US Version, Oxford Gray, Amazon Exclusive",
    "price": 209.98,
    "link": "https://www.amazon.com/SAMSUNG-Android-Included-Expandable-Exclusive/dp/B0CWS8MNW1"
  },
  {
    "description": "Deluxe 14PC Nonstick Cookware Sets, DUXANO Freshness-Maintained Pots and Pans with 9H Hardness 2-Layer Ceramic Coating, True Cool Handles, PFAS Free, Dishwasher Safe, All Cooktops & Induction Ready",
    "price": 198.89,
    "link": "https://www.amazon.com/Nonstick-DUXANO-Freshness-Maintained-Dishwasher-Induction/dp/B0D25GGFLS"
  }
]
``` 
For a `TimerTrigger` to work, you provide a schedule in the form of a [cron expression](https://en.wikipedia.org/wiki/Cron#CRON_expression). A cron expression is a string with 6 separate expressions which represent a given schedule via patterns. The pattern we use to represent every 5 minutes is `0 */5 * * * *`. This, in plain text, means: "When seconds is equal to 0, minutes is divisible by 5, for any hour, day of the month, month, day of the week, or year".

The deals website is generated by writing HTML content to the $web blob container. Additionally, I’ve created a static website that you can find <a href="https://github.com/Jimmy-Kroy/my-first-static-web-app/tree/main/src" target="_blank">here</a>. This static website utilizes the GetDealsOfTheDay function to populate the deals.

Note that you need to configure CORS for the Azure Function so that it allows the static website script to query the `GetDealsOfTheDay` REST API. You can accomplish this by adding the static website URL to the CORS list of the `GetDealsOfTheDay` function, as shown below. 

<img src="https://github.com/Jimmy-Kroy/25-days-of-serverless-solutions/blob/master/Day-10/Images/configure-cors-azure-function.png" alt="Configure CORS in Azure Function" width="600px">

The local.settings.json looks like this:
```json
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "TableStorageClient:ConnectionString": "<< Storage Account ConnectionString >>",
    "TableStorageClient:TableName": "Deals",
    "BlobStorageClient:ConnectionString": "<< Storage Account ConnectionString >>"
  }
}
``` 

## Resources/Tools Used

-   **[Blob bindings can't set ContentType and other properties](https://github.com/Azure/azure-functions-host/issues/364)**
-   **[Host a static website in Azure Storage](https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-static-website-how-to)**
-   **[Build your first static site with Azure Static Web Apps](https://learn.microsoft.com/en-us/azure/static-web-apps/getting-started)**

# Challenge 10: Timer Trigger

![A timer for shoppers](https://res.cloudinary.com/jen-looper/image/upload/v1575132447/images/challenge-10_d2nl4t.jpg)

## Daily Aggregated Deals

In Italy, children hang stockings on their fireplace so that an older woman named Befana can place their gifts inside of them. Along with the gifts, Befana also places candy or coal in each stocking based on how good they were that year.

This year has been really busy for Befana so she hired you as an assistant to help move things along faster. While Befana was shopping for candy, she lost her glasses. Her replacement glasses will arrive before the night of Epiphany, when children will be expecting their gifts. This is very unfortunate for Befana because she hasn't finished her gift shopping yet!

Befana is relying on you to finish shopping for the remaining gifts. Befana is really particular about getting a good deal on her shopping and expects you to be the same. Luckily, this is a major time of year for shoppers and deal seekers! But how do you keep track of all of these deals?Let's make a daily digest of current deals of interest!

In today's challenge, you'll create a tool that finds deals of the day from Twitter and adds them to a static web page. A Logic App is great serverless solution for this!

## Resources/Tools Used 🚀

- **[Azure Blob Storage](https://docs.microsoft.com/en-us/azure/storage/?WT.mc_id=25daysofserverless-github-cxa)**
- **[Logic Apps](https://docs.microsoft.com/en-us/azure/logic-apps/quickstart-create-first-logic-app-workflow/?WT.mc_id=25daysofserverless-github-cxa)**
- **[Recurrence Trigger for Logic Apps](https://docs.microsoft.com/en-us/azure/logic-apps/tutorial-build-schedule-recurring-logic-app-workflow?WT.mc_id=25daysofserverless-github-cxa)**
- **[Twitter Connector for Logic Apps](https://docs.microsoft.com/en-us/azure/connectors/connectors-create-api-twitter?WT.mc_id=25daysofserverless-github-cxa)**
- **[Blob Storage Connector for Logic Apps](https://docs.microsoft.com/en-us/azure/connectors/connectors-create-api-azureblobstorage?WT.mc_id=25daysofserverless-github-cxa#add-blob-storage-action)**
- **[Static website hosting in Azure Storage](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-static-website?WT.mc_id=25daysofserverless-github-cxa)**
- **[Visual Studio Code](https://code.visualstudio.com/?WT.mc_id=25daysofserverless-github-cxa)**

## Next Steps 🏃

Learn more about serverless with a Free Training!

-   ✅ **[Serverless Free Courses](https://docs.microsoft.com/learn/browse/?term=azure%20functions&WT.mc_id=25daysofserverless-github-cxa)**

## Important Resources ⭐️

- ✅ **[Azure Functions documentation](https://docs.microsoft.com/azure/azure-functions/?WT.mc_id=25daysofserverless-github-cxa)**
- ✅ **[Azure SDK for JavaScript Documentation](https://docs.microsoft.com/azure/javascript/?WT.mc_id=25daysofserverless-github-cxa)**
- ✅ **[Create your first function using Visual Studio Code](https://docs.microsoft.com/azure/azure-functions/functions-create-first-function-vs-code?WT.mc_id=25daysofserverless-github-cxa)**
- ✅ **[Free E-Book - Azure Serverless Computing Cookbook, Second Edition](https://azure.microsoft.com/resources/azure-serverless-computing-cookbook/?WT.mc_id=25daysofserverless-github-cxa)**

## Questions? Comments?

If you have any questions about the challenges, feel free to open an **[ISSUE HERE](https://github.com/microsoft/25-days-of-serverless/issues)**. Make sure to mention which challenge is problematic. We'll get back to you soon!
