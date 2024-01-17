async function convertCurrency() {
    // Fetch input values
    const amount = parseFloat(document.getElementById("amount").value);
    const currencyFrom = document.getElementById("currencyFrom").value;
    const currencyTo = document.getElementById("currencyTo").value;

    try {
        // API call as a put request with body as json
        const url = "http://localhost:5000/api/insert";
        const options = {
            method: "PUT",
            headers: {
                "Content-Type": "application/json" 
            },
            body: JSON.stringify({
                amount: amount,
                currencyFrom: currencyFrom,
                currencyTo: currencyTo
            })
        };

        const response = await fetch(url,options);
        if (response.ok) { // checks if response is accepted (okay)
            const result = await response.text();
            console.log(result)
            getTransaction();
        } else {
            alert('Error calling the API');
        }
    } catch (error) {
        console.error('Error:', error);
    }





    const result = amount + " " + currencyFrom + " = " + amount + " " + currencyTo;

    // Display the result
    document.getElementById("result").innerText = result;
}

async function getTransaction() {
    const url = "http://localhost:5000/api/select";
    const options = {
        method: "GET",

    };
    
    try {
        const response = await fetch(url, options);
        const result = await response.json(); // Parse response as JSON
        const listContainer = document.getElementById("transactionList");
        listContainer.innerHTML = ""; // refresher the list before filling it out again

        console.log(result);
        result.forEach(element => {
            const listItem = document.createElement("li");



            id = element["TransactionId"];
            date = element["TransactionDate"];
            to = element["CurrencyTo"];
            from = element["CurrencyFrom"];
            amount = element["Amount"];

            const transactionText = `ID: ${id} Date: ${date} From: ${from} To: ${to} Amount: ${amount}`;

            listItem.textContent = transactionText;

            listContainer.appendChild(listItem);


            console.log(transactionText);
            
        });


    } catch (error) {
        console.error(error);
        console.error("Error fetching data")
    }

}
async function fetchAvailableCurrencies() {

    const url = "https://currencyapi-net.p.rapidapi.com/currencies?output=JSON";
    const options = {
        method: "GET",
        headers: {
            "X-RapidAPI-Key": "347325257fmsh9ebd924422979c2p109af6jsn7cda0f2d9c36",
            "X-RapidAPI-Host": "currencyapi-net.p.rapidapi.com"
        }
    };
    
    try {
        const response = await fetch(url, options);
        const result = await response.json(); // Parse response as JSON
        console.log(result);

        // Check if the response is valid and contains currencies
        if (result.valid && result.currencies) {
            // Extract and display currency names
            const currencyOptions = Object.entries(result.currencies).map(([code, name]) => `<option value="${code}">${code}: ${name}</option>`);

            document.getElementById("currencyFrom").innerHTML = currencyOptions.join("");
            document.getElementById("currencyTo").innerHTML = currencyOptions.join("");

        } else {
            console.error("Invalid response structure")
        }
    } catch (error) {
        console.error(error);
        console.error("Error fetching data")
    }
}


function start()
{
    //fecthing all currencies from external api
    fetchAvailableCurrencies();
    getTransaction();

}

//execute data on load
window.onload = start;
