console.log("JS loaded. window.initialRecords:", window.initialRecords);  // Check if data is passed
let records = window.initialRecords || [];
console.log("Records initialized:", records);

let isLoggedIn = JSON.parse(localStorage.getItem('isLoggedIn')) || false;
let editingId = null;

// Check login on page load
if (!isLoggedIn) {
    showLogin();
} else {
    showDashboard();
}

function showLogin() {
    document.getElementById('loginPage').style.display = 'flex';
    document.getElementById('dashboardPage').style.display = 'none';
}

function showDashboard() {
    console.log("showDashboard called. Records length:", records.length);
    document.getElementById('loginPage').style.display = 'none';
    document.getElementById('dashboardPage').style.display = 'block';
    loadTable();
}

// Login form submission
document.getElementById('loginForm').addEventListener('submit', function (e) {
    e.preventDefault();
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    if (username === 'admin' && password === 'admin') { 
        isLoggedIn = true;
        localStorage.setItem('isLoggedIn', JSON.stringify(isLoggedIn));
        showDashboard();
    } else {
        alert('Invalid credentials');
    }
});

function logout() {
    isLoggedIn = false;
    localStorage.removeItem('isLoggedIn');
    showLogin();
}

// ========== TABLE FUNCTIONS ==========
function loadTable() {
    const tableBody = document.getElementById('tableBody');
    tableBody.innerHTML = '';  // Clear and rebuild
    for (let i = 0; i < records.length; i++) {
        const log = records[i];
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${log.logID}</td>
            <td>${log.cityName}</td>
            <td>${log.temperature}</td>
            <td>${log.humidity}</td>
            <td>${log.precipitation}</td>
            <td>${log.windSpeed}</td>
            <td>${log.weatherCode}</td>
            <td>${log.requestTimeStamp}</td>
            <td>
                <div class="action-buttons">
                    <button class="btn btn-edit" onclick="openEditModal(${log.LogID})">Edit</button>
                    <button class="btn btn-danger" onclick="deleteRecord(${log.logID})">Delete</button>
                </div>
            </td>
        `;
        tableBody.appendChild(row);
    }
}

// ========== MODAL FUNCTIONS ==========

function openEditModal(id) {
    editingId = id;
    const record = records.find(r => r.LogID === id);
    if (record) {
        document.getElementById('modalTitle').textContent = 'Edit Record';
        document.getElementById('cityName').value = record.cityName;
        document.getElementById('temperature').value = record.temperature;
        document.getElementById('humidity').value = record.humidity;
        document.getElementById('precipitation').value = record.precipitation;
        document.getElementById('windSpeed').value = record.windSpeed;
        document.getElementById('weatherCode').value = record.weatherCode;
        document.getElementById('modal').style.display = 'flex';
    }
}

function closeModal() {
    document.getElementById('modal').style.display = 'none';
    editingId = null;
}

// Form submission for add/edit
document.getElementById('dataForm').addEventListener('submit', handleSaveRecord);

async function handleSaveRecord(e) {
    e.preventDefault();
    const cityName = document.getElementById('cityName').value;
    const temperature = parseFloat(document.getElementById('temperature').value);
    const humidity = parseInt(document.getElementById('humidity').value);
    const precipitation = parseFloat(document.getElementById('precipitation').value);
    const windSpeed = parseFloat(document.getElementById('windSpeed').value);
    const weatherCode = document.getElementById('weatherCode').value;

    const recordData = {
        LogID: editingId,
        CityName: cityName,
        Temperature: temperature,
        Humidity: humidity,
        Precipitation: precipitation,
        WindSpeed: windSpeed,
        WeatherCode: weatherCode,
        RequestTimeStamp: records.find(r => r.LogID === editingId)?.RequestTimeStamp || new Date().toISOString()
    };

    console.log('Sending data:', recordData);  // Debug: Log what we're sending

    try {
        const response = await fetch('/api/AdminApi/Edit', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(recordData)
        });

        console.log('Response status:', response.status);  // e.g., 200, 400, 500
        console.log('Response ok:', response.ok);  // true if 200-299
        const responseText = await response.text();  // Get raw response as text
        console.log('Response text:', responseText);  // Should be JSON or error details

        if (response.ok) {
            const result = JSON.parse(responseText);  // Parse JSON manually
            if (result.success) {
                closeModal();
                await loadRecordsFromServer();
            } else {
                alert('Error: ' + (result.errors ? result.errors.join(', ') : result.message || 'Save failed'));
            }
        } else {
            alert('Server error (' + response.status + '): ' + responseText);
        }
    } catch (error) {
        console.error('Fetch error:', error);
        alert('Failed to save. Check console for details.');
    }
}

async function loadRecordsFromServer() {
    try {
        const response = await fetch('/api/AdminApi/GetRecords', { method: 'GET' });
        if (response.ok) {
            records = await response.json();
            loadTable();
        } else {
            console.error('Failed to load records');
        }
    } catch (error) {
        console.error('Error loading records:', error);
    }
}

document.addEventListener('DOMContentLoaded', async () => {
    if (isLoggedIn) {
        await loadRecordsFromServer();
    }
});



// ========== DELETE FUNCTION ==========
async function deleteRecord(id) {
    if (confirm('Are you sure you want to delete this record?')) {
        try {
            const response = await fetch(`/api/AdminApi/${id}`, {
                method: 'DELETE',  
                headers: { 'Content-Type': 'application/json' }
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const result = await response.json();
            if (result.success) {
                await loadRecordsFromServer();
            } else {
                alert('Error: ' + (result.message || 'Unknown error'));
            }
        } catch (error) {
            console.error('Fetch error:', error);
            alert('Network error occurred: ' + error.message);
        }
    }
}
// Close modal when clicking outside
document.addEventListener('click', function (event) {
    const modal = document.getElementById('modal');
    if (event.target === modal) {
        closeModal();
    }
});

// Load records from localStorage on page load (if available)
const storedRecords = JSON.parse(localStorage.getItem('adminRecords'));
if (storedRecords) {
    records = storedRecords;
    loadTable();
}