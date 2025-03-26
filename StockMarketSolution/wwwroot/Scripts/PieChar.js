// Get the canvas context
const ctx3 = document.getElementById('simple-pie-chart').getContext('2d');

// Sample data
const data = {
    labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple'], // Categories
    datasets: [{
        data: [12, 19, 3, 5, 8], // Random values
        backgroundColor: [
            '#FF6384', // Colors for each slice
            '#36A2EB',
            '#FFCE56',
            '#4BC0C0',
            '#9966FF'
        ],
        hoverBackgroundColor: [
            '#FF6384',
            '#36A2EB',
            '#FFCE56',
            '#4BC0C0',
            '#9966FF'
        ]
    }]
};

// Pie chart options
const options = {
    responsive: true,
    plugins: {
        legend: {
            display: true,
            position: 'top'
        },
        tooltip: {
            enabled: true
        }
    }
};

// Create the pie chart
const pieChart = new Chart(ctx, {
    type: 'pie',
    data: data,
    options: options
});
