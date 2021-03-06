var buildingList = [
	{
	    "name": "Wartburg College",
	    "lng": 4.679397940635681,
	    "lat": 51.801330542575634,
        "value": "Wartburg College"
	}, {
	    "name": "DVC Azzurro",
	    "lng": 4.683828949928284,
	    "lat": 51.79900173555325,
	    "value": "DVC Azzurro"
	}, {
	    "name": "DVC Romboutslaan",
	    "lng": 4.683378338813782,
	    "lat": 51.79811927867567,
        "value": "DVC Romboutslaan"
	}, {
	    "name": "Samenwerkingsgebouw",
	    "lng": 4.682798981666565,
	    "lat": 51.79846430146596,
	    "value": "Samenwerkingsgebouw"
	}, {
	    "name": "Drechtsteden college",
	    "lng": 4.681704640388489,
	    "lat": 51.798484206546384,
	    "value": "Drechtsteden college"
	}, {
	    "name": "Appartementen",
	    "lng": 4.680696129798889,
	    "lat": 51.799180878825474,
        "value": "Appartementen"
	}, {
	    "name": "Brandweerkazerne",
	    "lng": 4.681532979011536,
	    "lat": 51.79763491530391,
	    "value": "Brandweerkazerne"
	}, {
	    "name": "DVC Lilla",
	    "lng": 4.680996537208557,
	    "lat": 51.7982387099403,
	    "value": "DVC Lilla"
	}, {
	    "name": "DVC Marrone",
	    "lng": 4.680749773979187,
	    "lat": 51.7980330225656,
	    "value": "DVC Marrone"
	}, {
	    "name": "DVC Rosa",
	    "lng": 4.680739045143127,
	    "lat": 51.79775434785147,
	    "value": "DVC Rosa"
	}, {
	    "name": "DVC Verde",
	    "lng": 4.6806639432907104,
	    "lat": 51.79754202310376,
	    "value": "DVC Verde"
	}, {
	    "name": "DVC Giallo",
	    "lng": 4.680610299110413,
	    "lat": 51.79737614369895,
	    "value": "DVC Giallo"
	}, {
	    "name": "DVC Indaco",
	    "lng": 4.6802884340286255,
	    "lat": 51.7973628733202,
	    "value": "DVC Indaco"
	}, {
	    "name": "DVC Bianco",
	    "lng": 4.680127501487732,
	    "lat": 51.79819226448608,
	    "value": "DVC Bianco"
	}, {
	    "name": "DVC Ocra",
	    "lng": 4.679741263389587,
	    "lat": 51.797774253245315,
	    "value": "DVC Ocra"
	}, {
	    "name": "DVC Arcobaleno",
	    "lng": 4.679580330848694,
	    "lat": 51.79809273835169,
	    "value": "DVC Arcobaleno"
	}, {
	    "name": "DVC Celeste",
	    "lng": 4.679784178733826,
	    "lat": 51.798424491278745,
	    "value": "DVC Celeste"
	}, {
	    "name": "Duurzaamheidsfabriek",
	    "lng": 4.679387211799622,
	    "lat": 51.79735623812936,
	    "value": "Duurzaamheidsfabriek"
	}, {
	    "name": "Parkeerplaats Brandweerkazerne",
	    "lng": 4.681801199913025,
	    "lat": 51.79740931962874,
	    "value": "Parkeerplaats Brandweerkazerne"
	}, {
	    "name": "Parkeerplaats Ocra",
	    "lng": 4.678518176078796,
	    "lat": 51.7980330225656,
	    "value": "Parkeerplaats Ocra"
	}, {
	    "name": "Schippersinternaat Appartementen",
	    "lng": 4.678046107292175,
	    "lat": 51.79689177234445,
        "value": "Schippersinternaat Appartementen"
	}, {
	    "name": "Sporthal",
	    "lng": 4.679022431373596,
	    "lat": 51.79872970181585,
	    "value": "Sporthal"

	}, {
	    "name": "Bogermanschool",
	    "lng": 4.679376482963562,
	    "lat": 51.80072678934213,
        "value": "Bogermanschool"
	}, {
	    "name": "Parkeerplaats Duurzaamheidsfabriek",
	    "lng": 4.680191874504089,
	    "lat": 51.79716381717036,
	    "value": "Parkeerplaats Duurzaamheidsfabriek"
	}];
	
	var markersArray = [],
	infoWindow;


$(function(){
	$('#attachPicture input').change(changePhotoIcon);

	initializeGoogleMap();
    initializeBuildingSelect();
	getCurrentLocation();
    

	google.maps.event.addListener(map, "click", placeMarkerOnClick); 
});

function initializeGoogleMap() {
	var mapOptions = {
		zoom: 17,
		minZoom: 15
	};
	
	window.map = new google.maps.Map($('#map-canvas')[0], mapOptions);
}

function initializeBuildingSelect() {
    $('#buildingSelect').empty();
    $.each(buildingList, function() {
        $('#buildingSelect').append('<option val="' + this.name + '">' + this.name + '</option>');
    });
}

function setCurrentLocation(position) {
	var pos = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);

	infoWindow = new google.maps.InfoWindow({
		map: map,
		position: pos,
		content: 'Hier bevind je je nu'
	});

	map.setCenter(pos);
	var nearestLocationArray = getNearestLocations(pos);
    setSuggestions(nearestLocationArray);
}

function getCurrentLocation() {
	if(!navigator.geolocation) {
		handleNoGeolocation(false);
		return;
	}

	navigator.geolocation.getCurrentPosition(setCurrentLocation, function() {
		handleNoGeolocation(true);
	});			
}

function changePhotoIcon(){
	if (this.files && this.files[0]) {
		$('#attachPicture').css('background-image', 'url("images/Picture-Ok-Icon.png")');
	}
	else{
		$('#attachPicture').css('background-image', 'url("images/Camera-Icon.png")');			
	}
}

function handleNoGeolocation(errorFlag) {
	var content = 'Het ophalen van locatie is mislukt. Selecteer het dichtsbijzijnde gebouw.';

	var options = {
		map: map,
		position: new google.maps.LatLng(51.798265236388616, 4.680490493774441),
		content: content
	};

	infowindow = new google.maps.InfoWindow(options);
	map.setCenter(options.position);
}

function placeMarkerOnClick (clickEvent) {
	var nearestLocationArray = placeMarker(clickEvent.latLng);
	setSuggestions(nearestLocationArray);
}

function placeMarker(location) {
    // first remove all markers if there are any
    deleteOverlays();

    var nearestLocationArray = getNearestLocations(location),
    	nearestLocation = nearestLocationArray[0];

    var marker = new google.maps.Marker({
        position: nearestLocation.latLng, 
        map: map
    });

    map.panTo(nearestLocation.latLng);
    markersArray.push(marker);

    return nearestLocationArray; 
}

function getNearestLocations(latLng) {
	var sortedArray = [];
	for (var i = buildingList.length - 1; i >= 0; i--) {
		var building = buildingList[i];

		building.id = i;
		building.latLng = new google.maps.LatLng(building.lat, building.lng);
		building.distanceBetween = google.maps.geometry.spherical.computeDistanceBetween(latLng, building.latLng);

		sortedArray.push(building);
	};

	return sortedArray.sort(distanceBetweenComparer).slice(0, 3);		
}

function distanceBetweenComparer(a, b) {
	if (a.distanceBetween < b.distanceBetween)
	   return -1;
	if (a.distanceBetween > b.distanceBetween)
	    return 1;
	return 0;
}

// Deletes all markers in the array by removing references to them
function deleteOverlays() {
	if(infoWindow){
		infoWindow.close();
		infowindow = new google.maps.InfoWindow();
	}

    if (markersArray) {
        for (i in markersArray) {
            markersArray[i].setMap(null);
        }
    	markersArray.length = 0;
    }
}

function setSuggestions(suggestionArray) {
	var $suggestions = $('#map-suggestions').empty();

	setBuildingSelectValue(suggestionArray[0]);

	for (var i = 0; i < suggestionArray.length; i++) {
		var listItem = $("<button></button>")
			.text(suggestionArray[i].name)
			.val(suggestionArray[i].id);

		$suggestions.append(listItem);
	};

	$suggestions.find('button').click(function(e){
		e.preventDefault();


		var buildingId = $(this).val(),
			building = buildingList[buildingId];

		buildingLocation = new google.maps.LatLng(building.lat, building.lng);

		$('#map-suggestions').empty();

	    setBuildingSelectValue(building);
		placeMarker(buildingLocation);

	});
}

function setBuildingSelectValue(building) {
    console.log(building);
    $('#Building').val(building.value);
}