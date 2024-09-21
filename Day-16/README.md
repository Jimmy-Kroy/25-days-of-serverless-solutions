# Challenge 16: Continuous Integration and Continuous Delivery for Azure Functions

![Challenge 16: Continuous Integration and Continuous Delivery for Azure Functions](https://res.cloudinary.com/jen-looper/image/upload/v1576271287/images/challenge-16_esubpm.jpg)

## The Challenge

We are only nine days away from Xmas... let the posadas begin!

It's the 16th of December, which can only mean one thing: Posadas are finally starting in Mexico! Everyone is already preparing for the following nine days of posadas and deciding on venues all across Mexico City for this festive tradition.

A couple of months back, Xanath offered to put together a list of hosts and locations so that all her friends and family had the details for each posada. With all the servers missing and so little time to collect the sites and inform everyone Xanath has asked some friends for help. They will all be working together to make a solution to help folks to find the location of the next posada.

Your challenge is to create a simple solution for Xanath's friends and family to find the locations of the upcoming posadas as well as the name of the person hosting. Since there will be several people working on the project and adding locations at the same time, you need to make sure that the solution is accordingly updated and deploy to reflect these changes

Tips:
To allow for the solution and data to be updated as fast as possible, the deployment should be made automatically after a Pull Request has been merged. You can achieve this using services like GitHub Actions or Azure Pipelines.
You can specify the locations in any way you prefer (i.e. addresses, latitude and longitude pairs). Still, you need to make sure that every place added adheres to the same format.
There are many ways in which you can implement this solution; we recommend you start with a simple one, implement your CI/CD pipeline and refine later.