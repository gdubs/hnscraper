# hnscraper
Right click on solution and restore nuget package

Solution is using .Net Core 4.6.1 

Press F5 to run the console app. 
  1. User will need to enter a number from 0 - 100.
  2. Anything outside of that scope will return an error and will be logged. 
      a. Logs can be found in the bin folder
  3. After processing json results should be in the bin folder


Error testing:
  1. I've placed a comment to force an error on invalid URLs. 
      a. Will try to refactor code to add unit tests if I have time. But in the meantime, uncommenting specific comments will be the quickest way to test loggin of invalid urls
  2. Validation was also added for some properties. Same as above, I've left comments for testing
  
 
 
Docker:
  1. No experience with docker, but will try to see how I can get it to work with the app.

