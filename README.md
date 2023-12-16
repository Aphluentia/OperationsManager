# OperationsManager   
Intermediary between the application and Kafka broker. Orchestrates operations, validates data, and ensures secure data processing. Implements multithreading for efficient operation processing.  

## Setup       
- docker build . -t operationsmanager    
- docker run --name OperationsManager -p 9020:443 -p 8020:80 -d operationsmanager  