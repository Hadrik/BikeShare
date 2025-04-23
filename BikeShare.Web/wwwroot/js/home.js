let map = L.map('map').setView([49.83429254048945, 18.161536490538477], 16);

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);

async function fillMap() {
    let stations = await fetch('api/stations');
    let stationsData = await stations.json();

    for (let station of stationsData) {
        let marker = L.marker([station.Latitude, station.Longitude]).addTo(map);
        
        marker.bindPopup(`
            <div>
                <h1>${station.Name}</h1>
                <p>Bikes: <span id="bikes-${station.Id}">loading...</span></p>
            </div>
        `);
        
        marker.on('popupopen', async () => {
            let bikesResp = await fetch(`api/stations/${station.Id}/bikes`);
            document.getElementById(`bikes-${station.Id}`).innerText = await bikesResp.json();
        });
        
        marker.on('click', () => {
            map.panTo(marker.getLatLng());
        });
    }
}

fillMap();
