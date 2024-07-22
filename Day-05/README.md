# Solution
I created one Azure Function that accepts a Json file as input and returns a Json file as output. I used the free tier of the Translator Text Cognitive Service.

## Resources/Tools Used 🚀

-   **[Translator service](https://learn.microsoft.com/en-us/azure/ai-services/translator/quickstart-text-rest-api)**
-   **[Sentiment analysis](https://learn.microsoft.com/en-us/azure/ai-services/language-service/sentiment-opinion-mining/quickstart)**

## The end point:

### DetermineSentimentOfLetter
You can analyze one or more letters in one Rest api call, you need to use the JSON array notation as shown in the JSON request below. 

```json
DetermineSentimentOfLetter: [POST] http://localhost:7258/api/DetermineSentimentOfLetter
[ 
    {
        "sender": "tracy",
        "text": "我讨厌饭"
    }
]
``` 
###### _JSON Example of one letter that is sent to the endpoint._

```json
[
    {
        "sender": "Adam",
        "text": "my little brother is so annoying and stupid"
    },
    {
        "sender": "Adam",
        "text": "I really like the bike I got for a present"
    },
    {
        "sender": "Adam",
        "text": "The food is really bad at home, I'd rather go to McDonalds"
    }
]

``` 
###### _JSON Example of multiple letters that are sent to the endpoint._

When successful the endpoint will respond with a status 200 OK. In addition a JSON file is returned that contains the analysis of the letters. 
```json
[
    {
        "sender": "Adam",
        "text": "my little brother is so annoying and stupid",
        "language": "en",
        "translation": "my little brother is so annoying and stupid",
        "overall_sentiment_score": "Naughty",
        "positive_sentiment": 0.0,
        "negative_sentiment": 1.0,
        "neutral_sentiment": 0.0
    },
    {
        "sender": "Adam",
        "text": "I really like the bike I got for a present",
        "language": "en",
        "translation": "I really like the bike I got for a present",
        "overall_sentiment_score": "Nice",
        "positive_sentiment": 0.79,
        "negative_sentiment": 0.01,
        "neutral_sentiment": 0.2
    },
    {
        "sender": "Adam",
        "text": "The food is really bad at home, I'd rather go to McDonalds",
        "language": "en",
        "translation": "The food is really bad at home, I'd rather go to McDonalds",
        "overall_sentiment_score": "Naughty",
        "positive_sentiment": 0.0,
        "negative_sentiment": 0.92,
        "neutral_sentiment": 0.08
    }
]

``` 
###### _JSON reply from the DetermineSentimentOfLetter endpoint containing the sentiment info._

# Challenge 5: Smart Apps

![A letter writing challenge](https://res.cloudinary.com/jen-looper/image/upload/v1575132446/images/challenge-5_ervxzc.jpg)

## Resources/Tools Used 🚀

-   **[Visual Studio Code](https://code.visualstudio.com/?WT.mc_id=25daysofserverless-github-cxa)**
-   **[Postman](https://www.getpostman.com/downloads/)**
-   **[Azure Functions Extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions&WT.mc_id=25daysofserverless-github-cxa)**

## Naughty or Nice

It's freezing cold up here on the North Pole, which normally makes it the ideal place to host a server farm. But today Santa's elves are freaking out!

Children all over the world write Santa letters to say what they want for Christmas. The elves had scripts running locally on the server farm to process the letters but without the missing servers this is no longer possible. Santa could translate manually, but he won't be able to get through all the letters in time!

Write a serverless application that helps Santa figure out if a given child is being naughty or nice based on what they've said. You'll likely need to detect the language of the correspondence, translate it, and then perform sentiment analysis to determine whether it's naughty or nice.

Have a look at the API https://aka.ms/holiday-wishes to find a sample of messages to validate whether your solution will work for Santa and his elves.

## Next Steps 🏃

Learn more about serverless with a Free Training!

-   ✅ **[Serverless Free Courses](https://docs.microsoft.com/learn/browse/?term=azure%20functions&WT.mc_id=25daysofserverless-github-cxa)**

## Important Resources ⭐️

Here include all the important features related to the challenges that are integrated into microsoft.docs. Ex.:

-   ✅ **[Azure Functions documentation](https://docs.microsoft.com/azure/azure-functions/?WT.mc_id=25daysofserverless-github-cxa)**
-   ✅ **[Azure SDK for JavaScript Documentation](https://docs.microsoft.com/azure/javascript/?WT.mc_id=25daysofserverless-github-cxa)**
-   ✅ **[Create your first function using Visual Studio Code](https://docs.microsoft.com/azure/azure-functions/functions-create-first-function-vs-code?WT.mc_id=25daysofserverless-github-cxa)**
-   ✅ **[Free E-Book - Azure Serverless Computing Cookbook, Second Edition](https://azure.microsoft.com/resources/azure-serverless-computing-cookbook/?WT.mc_id=25daysofserverless-github-cxa)**

## Questions? Comments? ❓

If you have any questions about the challenges, feel free to open an **[ISSUE HERE](https://github.com/microsoft/25-days-of-serverless/issues)**. We'll get back to you soon!
