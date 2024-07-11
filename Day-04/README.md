# Solution
I created five Azure Functions for the API endpoints. All connected to a free tier of CosmosDB in Azure. 

## All the end points:

### AddFoodDishes
You can add one or multiple dishes in one api call, you need to use the JSON array notation as shown in the JSON request below. 

```json
AddFoodDishes: [POST] http://localhost:7108/api/PotluckManager/AddFoodDishes
[
    {
        "GuestName": "Chantal Disney",
        "Dish": "Broccoli, Grape, And Pasta Salad.",
        "Amount": "Four person salad.",
        "IsVegan": true
    }
]
``` 
###### _JSON Example of one dish that is sent to the endpoint._


```json
[
    {
        "GuestName": "Joe Statum",
        "Dish": "Classic Potato Salad.",
        "Amount": "Two people.",
        "IsVegan": true
    },
    {
        "GuestName": "Robert Lawrence",
        "Dish": "Classic Potato Salad with Bacon.",
        "Amount": "Five people.",
        "IsVegan": false
    }
]
``` 
###### _JSON Example of multiple dishes that are sent to the endpoint._

When successful the endpoint will respond with a status 200 OK. In addition a JSON file is returned that contains the id of every food dish that was processed. The id in the JSON file can be used to  retrieve, update or delete the registered food dish.   

```json
[
    {
        "Id": "e5526eb1-ebce-4fbf-b898-d62dda1dfc10",
        "CreationTime": "2024-07-11T19:48:47.3278296+02:00",
        "GuestName": "Chantal Disney",
        "Dish": "Broccoli, Grape, And Pasta Salad.",
        "Amount": "Four person salad.",
        "IsVegan": true
    }
]

``` 
###### _JSON reply from the AddFoodDishes endpoint containing the food dish id._

### DeleteFoodDishById

```json
DeleteFoodDishById: [DELETE] http://localhost:7108/api/PotluckManager/DeleteFoodDishById/{id}
```
When successful the endpoint will respond with a status 200 OK.

### GetAllFoodDishes

```json
GetAllFoodDishes: [GET] http://localhost:7108/api/PotluckManager/GetAllFoodDishes
```
When successful the endpoint will respond with a status 200 OK. In addition a JSON file is returned that contains all the records as shown below.

```json
[
    {
        "Id": "1c1b4654-d9f3-4976-97af-4c173011e8da",
        "CreationTime": "2024-07-11T20:12:30.62112+02:00",
        "GuestName": "Kyle Reese",
        "Dish": "Macaroni And Cheese.",
        "Amount": "For about six hungry persons!",
        "IsVegan": false
    },
    {
        "Id": "b8fef3c5-2069-4090-ae40-1852bf53d1c7",
        "CreationTime": "2024-07-11T20:12:30.6211233+02:00",
        "GuestName": "Jennifer van Dijk",
        "Dish": "Slow-Cooker Grape Jelly Meatballs.",
        "Amount": "Twenty balls of 100 gram each.",
        "IsVegan": false
    },
    {
        "Id": "9fafc7f0-e796-4453-85e2-04c18f6972bd",
        "CreationTime": "2024-07-11T20:12:30.6211266+02:00",
        "GuestName": "Chantal Disney",
        "Dish": "Broccoli, Grape, And Pasta Salad.",
        "Amount": "Four person salad.",
        "IsVegan": true
    }
]

```
###### _JSON reply from the GetAllFoodDishes endpoint containing all the records._

### GetFoodDishById

```json
GetFoodDishById: [GET] http://localhost:7108/api/PotluckManager/GetFoodDishById/{id}
```
When successful the endpoint will respond with a status 200 OK. In addition a JSON file is returned that contains the requested records as shown below. Note that a single record is returned, hence there is no JSON array returned but a single JSON object.   
```json
{
    "Id": "9fafc7f0-e796-4453-85e2-04c18f6972bd",
    "CreationTime": "2024-07-11T20:12:30.6211266+02:00",
    "GuestName": "Chantal Disney",
    "Dish": "Broccoli, Grape, And Pasta Salad.",
    "Amount": "Four person salad.",
    "IsVegan": true
}
```
###### _JSON reply from the GetFoodDishById endpoint containing the requested record._

### UpdateFoodDish

```json
UpdateFoodDish: [PUT] http://localhost:7108/api/PotluckManager/UpdateFoodDish
```
Updates are specified in a JSON format as shown in the example below. The JSON file needs to contain the food dish id and the fields that you want to change. Fields that you don't want to change can be omitted from the JSON request. When successful the endpoint will respond with a status 200 OK.

```json
    {
        "Id": "e5526eb1-ebce-4fbf-b898-d62dda1dfc10",
        "GuestName": "Chantal Disney and husband",
        "Dish": "Broccoli, Grape, And Pasta Salad and chicken wings.",
        "Amount": "Ten persons.",
        "IsVegan": false
    }
```
###### _JSON request to update a food dish record._

# Challenge 4: API Endpoint

![Ezra and his dinner dilemma](https://res.cloudinary.com/jen-looper/image/upload/v1575132446/images/challenge-4_shxtjs.jpg)

Here in Brooklyn, NY, Ezra wants to have a big holiday potluck before everyone travels home for the holidays! His tiny apartment can barely fit everyone in, but it's a cozy way to celebrate with friends. He usually uses an online spreadsheet to coordinate who's bringing what, to make sure there's varieties of food to meet all dietary needs.

But the grinch stole all the servers! So Ezra can't do that this year.

Build an HTTP API that lets Ezra's friends add food dishes they want to bring to the potluck, change or remove them if they need to (plans change!), and see a list of what everybody's committed to bring.

## Resources/Tools Used 🚀

-   **[Visual Studio Code](https://code.visualstudio.com/?WT.mc_id=25daysofserverless-github-gllemos)**
-   **[Node.js](https://nodejs.org/en/)**
-   **[Postman](https://www.getpostman.com/)**
-   **[MongoDB Community Server](https://www.mongodb.com/download-center/community)**
-   **[MongoDB Compass GUI](https://www.mongodb.com/download-center/compass)**
-   **[Azure Functions for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions&WT.mc_id=25daysofserverless-github-cxa)**
-   **[Azure Functions Core Tools](https://docs.microsoft.com/azure/azure-functions/functions-run-local?WT.mc_id=25daysofserverless-github-cxa)**

## Next Steps 🏃

Learn more about serverless with a Free Training!

-   ✅ **[Serverless Free Courses](https://docs.microsoft.com/learn/browse/?term=azure%20functions&WT.mc_id=25daysofserverless-github-cxa)**

## Important Resources ⭐️

-   ✅ **[Azure Functions documentation](https://docs.microsoft.com/azure/azure-functions/?WT.mc_id=25daysofserverless-github-cxa)**
-   ✅ **[Azure SDK for JavaScript Documentation](https://docs.microsoft.com/azure/javascript/?WT.mc_id=25daysofserverless-github-cxa)**
-   ✅ **[Create your first function using Visual Studio Code](https://docs.microsoft.com/azure/azure-functions/functions-create-first-function-vs-code?WT.mc_id=25daysofserverless-github-cxa)**
-   ✅ **[Free E-Book - Azure Serverless Computing Cookbook, Second Edition](https://azure.microsoft.com/resources/azure-serverless-computing-cookbook/?WT.mc_id=25daysofserverless-github-cxa)**

## Questions? Comments? ❓

If you have any questions about the challenges, feel free to open an **[ISSUE HERE](https://github.com/microsoft/25-days-of-serverless/issues)**. We'll get back to you soon!
