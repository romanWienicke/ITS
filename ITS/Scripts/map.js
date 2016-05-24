// globals
var map, mapHeight, geocoder, poly;
var image = "/images/planeicon.png";
var departureImage = "/images/departure.png";
var destinationImage = "/images/destination.png";
var departure = null, destination = null;
var setCenter = true;
var mapOptions;

// endcode to save path
function encodePath(path) {
    var encodeString = google.maps.geometry.encoding.encodePath(path);
    $("#MapPath").val(encodeString);
}

function checkAirports(fieldId) {
    var simpleMarker = fieldId.toLowerCase();
    var value = $("#" + fieldId + "AirportId option:selected").text();
    if (value.length > 0) {
        geocodeAddressAndAddMarker(value, simpleMarker);
    }
}


// create the initial polyline
function createPoly()
{
    poly = new google.maps.Polyline({
        strokeColor: '#FF0000',
        strokeOpacity: 1.0,
        strokeWeight: 3
    });
    // add the polyline to the map
    poly.setMap(map);
}

function createMarker(latLng) {
    // Add a new marker at the new plotted point on the polyline.
    

    return new google.maps.Marker({
        
        position: latLng,
        map: map,
        icon: image
    });
    // add click to remove
    
}

function setupMap(disableUI) {
    if (!disableUI)
    {
        disableUI = false;
    }

    if ($("#map_cavans").length < 1)
    {
        return;
    }
    mapOptions = {
        mapTypeId: google.maps.MapTypeId.HYBRID,
        zoom: 7,
        scrollwheel: false,
        disableDefaultUI: disableUI,
    };

    map = new google.maps.Map(document.getElementById("map_cavans"), mapOptions);


    // create the initial polyline
    createPoly();

    checkAirports("Departure");
    checkAirports("Destination");
}

// Handles click events on a map, and adds a new point to the Polyline.
function onAddMarker(event) {
    addMarker(event.latLng);
}


function addMarker(latLng) {
    var path = poly.getPath();
    // add to polyline

    //path.push(latLng);
    var index = path.length - 1;
    path.insertAt(index, latLng);
    // endode to save path
    encodePath(path);

    var marker = createMarker(latLng);
    marker.addListener('click', onRemoveMarker);
}


// remove handler
function onRemoveMarker(event) {
    removeMarker(this);
}

function removeMarker(marker) {
    marker.setMap(null);
    var path = poly.getPath();
    var index = path.j.indexOf(marker.position);
    path.removeAt(index);
}

// find address and add departure or destination
function geocodeAddressAndAddMarker(address, type) {
    // if not exists create
    if (!geocoder) {
        geocoder = new google.maps.Geocoder();
    }

    // geocode address and get result
    geocoder.geocode({ 'address': address }, function (results, status) {
        // geocode worked
        if (status === google.maps.GeocoderStatus.OK) {
            // get marker if exists

            marker = (type == "departure") ? departure : destination;
            icon = (type == "departure") ? departureImage : destinationImage;
            // if marker exists / remove it and clean up
            if (marker) {
                removeMarker(marker);
            }
            // get the location
            var location = results[0].geometry.location;

            if (setCenter) {
                // set the map and add the marker
                map.setCenter(location);
            }
            marker = new google.maps.Marker({
                map: map,
                position: location,
                icon: icon
            });

            // get the polyline path
            var path = poly.getPath();
            var index = 0;
            // set it to the global variable
            if (type == "departure") {
                departure = marker;
            } else {
                destination = marker;
                // if it's the destination, get the last item of the path and insert it there
                index = path.length;
            }

            path.insertAt(index, location);
            encodePath(path);
        } else {
            // error message
            alert('Geocode was not successful for the following reason: ' + status);
        }

    });
}