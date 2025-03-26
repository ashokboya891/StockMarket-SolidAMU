// WebSocket message event handler
socket.addEventListener('message', (event) => {
    try {
        const eventData = JSON.parse(event.data);

        if (eventData.type === "error") {
            console.error(`Error: ${eventData.msg || "Unknown error"}`);
            return;
        }

        if (eventData.data) {
            const tradeData = eventData.data[0];
            const updatedPrice = tradeData.p; // Latest price
            const stockSymbol = tradeData.s; // Stock symbol

            // Update the pie chart
            updatePieChart(stockSymbol, updatedPrice);

            // Update the price UI (if needed)
            $(".price").text(updatedPrice.toFixed(2));
        }
    } catch (error) {
        console.error("Error processing WebSocket message:", error);
    }
});
