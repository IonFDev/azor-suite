const API_URL = 'http://localhost:8080/api';

async function testApi() {
    const response = await fetch(`${API_URL}/test`);

    if (!response.ok) {
        throw new Error(`HTTP ${response.status}`);
    }

    const data = await response.json();

    document.getElementById('result').textContent =
        data.application;
}

testApi().catch(error => {
    document.getElementById('result').textContent =
        `Error: ${error.message}`;
});