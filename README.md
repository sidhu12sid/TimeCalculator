This is a simple backend application to find the time by giving the punchin and punchout time and dates

API curl

curl --location --request POST 'https://timecalculator-deebf4apfbhvenca.canadacentral-01.azurewebsites.net/api/Punch/calculate?punchData=26-Mar-2025%098%3A53%3A03%20AM%0926-Mar-2025%091%3A05%3A11%20PM%09%0A26-Mar-2025%091%3A39%3A11%20PM%0926-Mar-2025%094%3A12%3A09%20PM%09%0A26-Mar-2025%094%3A28%3A10%20PM%09%09%09%0A' \
--header 'accept: */*' \
--header 'Content-Type: application/json-patch+json'
