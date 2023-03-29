var source, destination;
var directionsDisplay;
var latitude;
var longitude;
var latitude1;
var longitude1;
var locations = [];
var sourcedetail = [];
var destinationdetail = [];
var PanelXml = [];
var panel = [];
var inverter = [];
var InverterXml = [];
var dateaa = '';
var jsonvarSoldByDate = '';
var jsonvarJobSoldBy = '';
var ElectrianAddressJson = [];
var OwnerAddressJson = [];
var InstallationJson = [];
var InstallerJson = [];
var OwnerPostcodeFromjson;
var InstallationPostcodeFromjson;
var OwnerEmailFromJson;
var OwnerFirstNameFromJson;
var OwnerLastNameFromJson;
var InstallerEmailFromJson;
var InstallerFirstNameFromJson;
var InstallerLastNameFromJson;

var batteryXml = [];

$("#BasicDetails_JobStage").change(function () {
    var jobStage = $("#BasicDetails_JobStage option:selected").text();
    $('#BasicDetails_CurrentJobStage').val(jobStage);
});
$("#BasicDetails_Priority").change(function () {
    var jobPriority = $("#BasicDetails_Priority option:selected").text();
    $('#BasicDetails_CurrentPriority').val(jobPriority);
});
$("#SSCID").change(function () {
    var jobPriority = $("#SSCID option:selected").text();
    $('#BasicDetails_SSCName').val(jobPriority);
});

var fillBasicAddress = function () {
    if ($("#BasicDetails_JobID").val() != "0") {
        if ($("#JobElectricians_Town").val().trim() != "" && $("#JobElectricians_Town").val() != undefined && $("#JobElectricians_Town").val() != null) {
            if ($("#JobElectricians_UnitTypeID").find('option').length > 0 && $("#JobElectricians_PostalAddressID").find('option').length > 0 && $("#JobElectricians_StreetTypeID").find('option').length > 0) {
                var UnitTypeId = $("#JobElectricians_UnitTypeID [value=" + JobElectricians_UnitTypeID + "]").text();

                var UnitNumber = $("#JobElectricians_UnitNumber").val();
                var StreetNumber = $("#JobElectricians_StreetNumber").val();
                var StreetName = $("#JobElectricians_StreetName").val();
                var StreetTypeId = $("#JobElectricians_StreetTypeID").find("option:selected").text();
                var PostalAddressID = $("#JobElectricians_PostalAddressID [value=" + JobElectricians_PostalAddressID + "]").text();
                var PostalDeliveryNumber = $("#JobElectricians_PostalDeliveryNumber").val();
                var Town = $("#JobElectricians_Town").val();
                var State = $("#JobElectricians_State").val();
                var PostCode = $("#JobElectricians_PostCode").val();
                //bansi
                var companyName = $('#JobElectricians_CompanyName').val();
                var firstName = $('#JobElectricians_FirstName').val();
                var lastName = $('#JobElectricians_LastName').val();
                var email = $('#JobElectricians_Email').val();
                var phone = $('#JobElectricians_Phone').val();
                var mobile = $('#JobElectricians_Mobile').val();
                var licenseNumber = $('#JobElectricians_LicenseNumber').val();

                if ($("#JobElectricians_AddressID").val() == 1) {
                    if (UnitNumber != "") {
                        address = UnitTypeId + ' ' + UnitNumber + "/" + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                        address = address.replace("Select", "");
                    } else {
                        address = UnitTypeId + ' ' + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                        address = address.replace("Select", "");
                    }
                    ElectrianAddressJson.push({ PostalAddressType: $("#JobElectricians_AddressID").val(), UnitType: $("#JobElectricians_UnitTypeID").val(), UnitNumber: $("#JobElectricians_UnitNumber").val(), StreetNumber: $("#JobElectricians_StreetNumber").val(), StreetName: $("#JobElectricians_StreetName").val(), StreetType: $("#JobElectricians_StreetTypeID").val(), Town: $("#JobElectricians_Town").val(), State: $("#JobElectricians_State").val(), PostCode: $("#JobElectricians_PostCode").val(), CompanyName: companyName, FirstName: firstName, LastName: lastName, Email: email, Phone: phone, Mobile: mobile, LicenseNumber: licenseNumber });
                }
                else {
                    address = PostalAddressID + ' ' + PostalDeliveryNumber + ', ' + Town + ' ' + State + ' ' + PostCode;
                    ElectrianAddressJson.push({ PostalAddressType: $("#JobElectricians_AddressID").val(), PostalAddressID: $("#JobElectricians_PostalAddressID").val(), PostalDeliveryNumber: $("#JobElectricians_PostalDeliveryNumber").val(), Town: $("#JobElectricians_Town").val(), State: $("#JobElectricians_State").val(), PostCode: $("#JobElectricians_PostCode").val(), CompanyName: companyName, FirstName: firstName, LastName: lastName, Email: email, Phone: phone, Mobile: mobile, LicenseNumber: licenseNumber });
                }
                address = address.replace("Select", "");
            }
        }

        $("#txtBasicAddress").val($('#JobElectricians_FirstName').val() + ' ' + $('#JobElectricians_LastName').val());

    }
};



var fillInstallationAddress = function () {

    if ($("#BasicDetails_JobID").val() != "0") {
        if ($("#JobInstallationDetails_Town").val().trim() != "" && $("#JobInstallationDetails_Town").val() != undefined && $("#JobInstallationDetails_Town").val() != null) {

            if ($("#JobInstallationDetails_UnitTypeID").find('option').length > 0 && $("#JobInstallationDetails_StreetTypeID").find('option').length > 0 && $("#JobInstallationDetails_PostalAddressID").find('option').length > 0) {
                var address;
                var UnitTypeId = $("#JobInstallationDetails_UnitTypeID").find("option:selected").text();
                var UnitNumber = $("#JobInstallationDetails_UnitNumber").val();
                var StreetNumber = $("#JobInstallationDetails_StreetNumber").val();
                var StreetName = $("#JobInstallationDetails_StreetName").val();
                var StreetTypeId = $("#JobInstallationDetails_StreetTypeID").find("option:selected").text();
                var PostalAddressID = $("#JobInstallationDetails_PostalAddressID").find("option:selected").text();
                var PostalDeliveryNumber = $("#JobInstallationDetails_PostalDeliveryNumber").val();
                var Town = $("#JobInstallationDetails_Town").val();
                var State = $("#JobInstallationDetails_State").val();
                var PostCode = $("#JobInstallationDetails_PostCode").val();
                if ($("#JobInstallationDetails_AddressID").val() == 1) {
                    if (UnitNumber != "") {
                        address = UnitTypeId + ' ' + UnitNumber + "/" + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                        address = address.replace("Select", "");
                    } else {
                        address = UnitTypeId + ' ' + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                        address = address.replace("Select", "");
                    }
                    InstallationJson.push({ PostalAddressType: $("#JobInstallationDetails_AddressID").val(), UnitType: $("#JobInstallationDetails_UnitTypeID").val(), UnitNumber: $("#JobInstallationDetails_UnitNumber").val(), StreetNumber: $("#JobInstallationDetails_StreetNumber").val(), StreetName: $("#JobInstallationDetails_StreetName").val(), StreetType: $("#JobInstallationDetails_StreetTypeID").val(), Town: $("#JobInstallationDetails_Town").val(), State: $("#JobInstallationDetails_State").val(), PostCode: $("#JobInstallationDetails_PostCode").val() });
                } else {
                    address = PostalAddressID + ' ' + PostalDeliveryNumber + ', ' + Town + ' ' + State + ' ' + PostCode;
                    InstallationJson.push({ PostalAddressType: $("#JobInstallationDetails_AddressID").val(), PostalAddressID: $("#JobInstallationDetails_PostalAddressID").val(), PostalDeliveryNumber: $("#JobInstallationDetails_PostalDeliveryNumber").val(), Town: $("#JobInstallationDetails_Town").val(), State: $("#JobInstallationDetails_State").val(), PostCode: $("#JobInstallationDetails_PostCode").val() });
                }
                InstallationPostcodeFromjson = $("#JobInstallationDetails_PostCode").val();
                address = address.replace("Select", "");
                if (address.trim() != "/ ," && address.trim() != "/Select ,") {
                    $("#JobInstallationDetails_AddressDisplay").val(address);
                    $("#txtAddress").val(address); $('#spantxtAddress').hide();
                }
            }
        }
    }
};
var fillOwnerAddress = function () {
    if ($("#BasicDetails_JobID").val() != "0") {
        if ($("#JobOwnerDetails_UnitTypeID").find('option').length > 0 && $("#JobOwnerDetails_PostalAddressID").find('option').length > 0 && $("#JobOwnerDetails_StreetTypeID").find('option').length > 0) {
            if ($("#JobOwnerDetails_AddressID").val() == 1) {
                var obj = { OwnerType: $("#JobOwnerDetails_OwnerType").val(), CompanyName: $("#JobOwnerDetails_CompanyName").val(), Phone: $('#JobOwnerDetails_Phone').val(), Mobile: $('#JobOwnerDetails_Mobile').val(), FirstName: $('#JobOwnerDetails_FirstName').val(), LastName: $('#JobOwnerDetails_LastName').val(), Email: $('#JobOwnerDetails_Email').val(), PostalAddressType: $("#JobOwnerDetails_AddressID").val(), UnitType: $("#JobOwnerDetails_UnitTypeID").val(), UnitNumber: $("#JobOwnerDetails_UnitNumber").val(), StreetNumber: $("#JobOwnerDetails_StreetNumber").val(), StreetName: $("#JobOwnerDetails_StreetName").val(), StreetType: $("#JobOwnerDetails_StreetTypeID").val(), Town: $("#JobOwnerDetails_Town").val(), State: $("#JobOwnerDetails_State").val(), PostCode: $("#JobOwnerDetails_PostCode").val() };
                OwnerAddressJson.push(obj);
            }
            else {
                OwnerAddressJson.push({ OwnerType: $("#JobOwnerDetails_OwnerType").val(), CompanyName: $("#JobOwnerDetails_CompanyName").val(), Phone: $('#JobOwnerDetails_Phone').val(), Mobile: $('#JobOwnerDetails_Mobile').val(), FirstName: $('#JobOwnerDetails_FirstName').val(), LastName: $('#JobOwnerDetails_LastName').val(), Email: $('#JobOwnerDetails_Email').val(), PostalAddressType: $("#JobOwnerDetails_AddressID").val(), PostalAddressID: $("#JobOwnerDetails_PostalAddressID").val(), PostalDeliveryNumber: $("#JobOwnerDetails_PostalDeliveryNumber").val(), Town: $("#JobOwnerDetails_Town").val(), State: $("#JobOwnerDetails_State").val(), PostCode: $("#JobOwnerDetails_PostCode").val() });

            }
            OwnerPostcodeFromjson = $("#JobOwnerDetails_PostCode").val();
            OwnerEmailFromJson = $('#JobOwnerDetails_Email').val();
            OwnerFirstNameFromJson = $("#JobOwnerDetails_FirstName").val();
            OwnerLastNameFromJson = $("#JobOwnerDetails_LastName").val();
        }
    }
};

var fillInstallerAddress = function () {
    if ($("#BasicDetails_JobID").val() != "0" && $('#BasicDetails_JobType').val() == "2") {
        if ($("#JobInstallerDetails_UnitTypeID").find('option').length > 0 && $("#JobInstallerDetails_PostalAddressID").find('option').length > 0 && $("#JobInstallerDetails_StreetTypeID").find('option').length > 0) {
            if ($("#JobInstallerDetails_AddressID").val() == 1) {
                InstallerJson.push({ Phone: $('#JobInstallerDetails_Phone').val(), Mobile: $('#JobInstallerDetails_Mobile').val(), FirstName: $('#JobInstallerDetails_FirstName').val(), Surname: $('#JobInstallerDetails_Surname').val(), Email: $('#JobInstallerDetails_Email').val(), PostalAddressType: $("#JobInstallerDetails_AddressID").val(), UnitType: $("#JobInstallerDetails_UnitTypeID").val(), UnitNumber: $("#JobInstallerDetails_UnitNumber").val(), StreetNumber: $("#JobInstallerDetails_StreetNumber").val(), StreetName: $("#JobInstallerDetails_StreetName").val(), StreetType: $("#JobInstallerDetails_StreetTypeID").val(), Town: $("#JobInstallerDetails_Town").val(), State: $("#JobInstallerDetails_State").val(), PostCode: $("#JobInstallerDetails_PostCode").val() });
            }
            else {
                InstallerJson.push({ Phone: $('#JobInstallerDetails_Phone').val(), Mobile: $('#JobInstallerDetails_Mobile').val(), FirstName: $('#JobInstallerDetails_FirstName').val(), Surname: $('#JobInstallerDetails_Surname').val(), Email: $('#JobInstallerDetails_Email').val(), PostalAddressType: $("#JobInstallerDetails_AddressID").val(), PostalAddressID: $("#JobInstallerDetails_PostalAddressID").val(), PostalDeliveryNumber: $("#JobInstallerDetails_PostalDeliveryNumber").val(), Town: $("#JobInstallerDetails_Town").val(), State: $("#JobInstallerDetails_State").val(), PostCode: $("#JobInstallerDetails_PostCode").val() });

            }
            InstallerEmailFromJson = $('#JobInstallerDetails_Email').val();
            InstallerFirstNameFromJson = $("#JobInstallerDetails_FirstName").val();
            InstallerLastNameFromJson = $("#JobInstallerDetails_Surname").val();
        }
    }
};


var fillJobStage = function () {
    $("#BasicDetails_JobStage").find("option").eq(0).remove();

};
function fillJobType() {
    if (BasicDetails_JobID == 0) {
        $("#BasicDetails_JobType").val('1').change();
        $('#BasicDetails_JobType option').first().remove();
    }
}

$("#noWarning").click(function () {
    $("#warning").modal('hide');
    if ($("#BasicDetails_JobID").val() != "0") {
        $("#BasicDetails_JobType").attr('disabled', 'disabled');
        $("#BasicDetails_JobType").change();
    }
});

$("#CloseWarning").click(function () {
    $("#STCwarning").modal('hide');
    if ($("#BasicDetails_JobID").val() != "0") {
        $("#BasicDetails_JobType").attr('disabled', 'disabled');
        $("#BasicDetails_JobType").change();
    }
});

$("#yesWarning").click(function () {
    $("#warning").modal('hide');
    SaveJob(jobSaveType);
});

function fillJobType() {
    if (BasicDetails_JobID == 0) {
        $("#BasicDetails_JobType").val('1').change();
        $('#BasicDetails_JobType option').first().remove();
    }
}
function clearSoldBy() {
    $('#popupSoldBy').modal('hide');

}
function SaveJob(saveType) {
    if (isAssignSSC == 'false') {
        $("#BasicDetails_SSCID").val(BasicDetails_SSCID);
    }
    else {
        if ($("#SSCID").val() != "" && $("#SSCID").val() > 0) {
            $("#BasicDetails_SSCID").val($("#SSCID").val());
        }
        else {
            $("#BasicDetails_SSCID").val('');
        }
    }
    if (isAssignSCO == 'false') {
        $("#BasicDetails_ScoID").val(BasicDetails_ScoID);
    }
    else {
        if ($("#ScoID").val() != "" && $("#ScoID").val() > 0) {
            $("#BasicDetails_ScoID").val($("#ScoID").val());
        }
        else {
            $("#BasicDetails_ScoID").val('');
        }
    }
    //Date Convert.

    EnableDropDownbyUsertype();
    $("#BasicDetails_JobType").removeAttr("disabled");
    var panelData = '';
    var xmlPanel = '';
    var inverterData = '';
    var xmlInverter = '';

    var batteryManufacturerData = [];
    batteryManufacturerData = JSON.parse(JSON.stringify(batteryXml));

    if (PanelXml.length > 0) {
        var jsonp = JSON.stringify(PanelXml);
        var sName = '';
        var obj = $.parseJSON(jsonp);
        $.each(obj, function () {
            sName += '<panel><Brand>' + htmlEncode(this['Brand']) + '</Brand>' + '<Model>' + htmlEncode(this['Model']) + '</Model>' +
             '<NoOfPanel>' + this['Count'] + '</NoOfPanel></panel>';
        });
        xmlPanel = '<Panels>' + sName + '</Panels>'
    }
    if (InverterXml.length > 0) {

        var jsonp = JSON.stringify(InverterXml);
        var sName = '';
        var obj = $.parseJSON(jsonp);
        $.each(obj, function () {
            sName += '<inverter><Brand>' + htmlEncode(this['Brand']) + '</Brand>' + '<Model>' + htmlEncode(this['Model']) + '</Model>' +
             '<Series>' + htmlEncode(this['Series']) + '</Series></inverter>';
        });
        xmlInverter = '<Inverters>' + sName + '</Inverters>';
    }

    $("#panelXml").val(xmlPanel);
    $("#inverterXml").val(xmlInverter);

    var data = JSON.stringify($('form').serializeToJson());
    var objData = JSON.parse(data);

    objData.JobSTCDetails.batterySystemPartOfAnAggregatedControl = $("#JobSTCDetails_batterySystemPartOfAnAggregatedControl").val();
    objData.JobSTCDetails.changedSettingOfBatteryStorageSystem = $("#JobSTCDetails_changedSettingOfBatteryStorageSystem").val();

    if (batteryManufacturerData.length > 0) {

        for (var i = 0; i < batteryManufacturerData.length; i++) {
            delete batteryManufacturerData[i]["JobBatteryManufacturerId"];
        }
        objData.lstJobBatteryManufacturer = batteryManufacturerData;
    }

    var postUrl = '';
    if (isAddJob == 'True') {
        postUrl = urlCreate;
    } else if (isEditjob == 'True') {
        postUrl = urlEdit;
    }

    var postUrl = '';
    if (isAddJob == 'True') {
        postUrl = urlCreate;
    } else if (isEditjob == 'True') {
        postUrl = urlEdit;
    }

    data = JSON.stringify(objData)

    $.ajax({
        url: postUrl,
        type: "POST",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8', // Not to set any content header
        processData: false, // Not to process data
        data: data,
        cache: false,
        async: true,
        success: function (result) {

            if (result.insert || result.update) {

                if ($("#BasicDetails_JobType").val() == 1) {
                    if (result.STCValue > 0)
                        $("#JobSystemDetails_CalculatedSTC").val(result.STCValue);
                    else
                        $("#JobSystemDetails_CalculatedSTC").val("");
                }
                else {
                    if (result.STCValue > 0)
                        $("#JobSystemDetails_CalculatedSTCForSWH").val(result.STCValue);
                    else
                        $("#JobSystemDetails_CalculatedSTCForSWH").val("");
                }

            }

            if (result.insert) {
                $("#successMsgRegion").html(closeButton + " Job has been saved successfully.");
                $("#successMsgRegion").show();
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');
                if (saveType == 0) {
                    window.location.href = urlIndex + "/" + result.id;
                } else { window.location.href = urlCreate; }
                DisableDropDownbyUsertype();
            } else if (result.update) {
                $("#successMsgRegion").html(closeButton + " Job has been updated.");
                $("#successMsgRegion").show();
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');
                $('#dvDocuments').load(url_Documents + result.id);
                DisableDropDownbyUsertype();
            } else if (result.error) {
                //alert(result.error);
                $("#errorMsgRegion").html(closeButton + result.errorMessage);
                //$("#errorMsgRegion").html(closeButton + "The field cannot contain symbols like < > ^ [ ].");
                $("#errorMsgRegion").show();
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');
                DisableDropDownbyUsertype();
            } else {
                $("#errorMsgRegion").html(closeButton + " Job not save.");
                $("#errorMsgRegion").show();
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');
                DisableDropDownbyUsertype();
            }
            if (result.insert || result.update) {
                if ($("#BasicDetails_GSTDocument").val() != '' && $("#BasicDetails_GSTDocument").val() != undefined && $("#BasicDetails_GSTDocument").val() != null)
                    $("#BasicDetails_GSTDocument").attr('OldFileName', $("#BasicDetails_GSTDocument").val());
                else
                    $("#BasicDetails_GSTDocument").attr('OldFileName', '');
            }
        }
    });
    $("#BasicDetails_JobType").attr('disabled', 'disabled');
}
function loadMap() {
    var map = new google.maps.Map(document.getElementById('dvMap'));
    directionsService = new google.maps.DirectionsService();
    //google.maps.event.addDomListener(window, 'load', function () {
    new google.maps.places.SearchBox(document.getElementById('txtSource'));
    new google.maps.places.SearchBox(document.getElementById('txtDestination'));


    // });
    directionsDisplay = new google.maps.DirectionsRenderer({
        draggable: false
    });

    google.maps.event.addListener(directionsDisplay, 'directions_changed', function () {

        directions = directionsDisplay.getDirections();
        var distance = directions.routes[0].legs[0].distance.text;
        var duration = directions.routes[0].legs[0].duration.text;
        var source = directions.routes[0].legs[0].start_address;
        var dest = directions.routes[0].legs[0].end_address;
        var dvDistance = document.getElementById("dvDistance");
        dvDistance.innerHTML = "";
        dvDistance.innerHTML += "Distance: " + distance + "   ";
        dvDistance.innerHTML += "Duration:" + duration;
        $("#txtSource").val(source);
        $("#txtDestination").val(dest);
    });

}
function GetRoute() {
    $("#dvDistance").html('');
    $("#errorMsgRegionMap").hide();
    source = document.getElementById("txtSource").value;
    destination = document.getElementById("txtDestination").value;

    if (source != "" && destination != "") {
        var India = new google.maps.LatLng(51.508742, -0.120850);
        var mapOptions = {
            zoom: 4,
            center: India
        };
        map = new google.maps.Map(document.getElementById('dvMap'), mapOptions);

        directionsDisplay.setMap(map);
        //*********DIRECTIONS AND ROUTE**********************//
        var request = {
            origin: source,
            destination: destination,
            travelMode: google.maps.TravelMode.DRIVING
        };
        directionsService.route(request, function (response, status) {
            if (status == google.maps.DirectionsStatus.OK) {
                directionsDisplay.setDirections(response);
            }
            else {
                createMarker(destination, 25.2744, 133.7751);
                $("#errorMsgRegionMap").html(closeButton + " Both Source and Destination  are not in same country.");
                $("#errorMsgRegionMap").show();
                //$("#errorMsgRegionMap").fadeOut(5000);


            }

        });
    }
    else {
        $("#errorMsgRegionMap").html(closeButton + " Both Source and Destination  address are  required");
        $("#errorMsgRegionMap").show();
        //$("#errorMsgRegionMap").fadeOut(5000);
    }
}
function GetLocation() {

    latitude = '';
    longitude = '';
    latitude1 = '';
    longitude1 = '';

    $("#dvDistance").html('');
    $("#errorMsgRegionMap").hide();
    //$("#dvMap").html('');
    locations = [];
    source = document.getElementById("txtSource").value;
    destination = document.getElementById("txtDestination").value;


    geocoder = new google.maps.Geocoder();

    if (source != "" || destination != "") {

        if (source != "") {
            geocoder.geocode({ 'address': source }, function (results, status) {

                if (status == google.maps.GeocoderStatus.OK) {
                    latitude = results[0].geometry.location.lat();
                    longitude = results[0].geometry.location.lng();
                    sourcedetail = [source, latitude, longitude];
                    locations.push(sourcedetail);
                    GetLocationOnMap(latitude, longitude);
                }
                else {
                    $("#errorMsgRegionMap").html(closeButton + "Invalid source Address.");
                    $("#errorMsgRegionMap").show();
                    //$("#errorMsgRegionMap").fadeOut(5000);
                    createMarker('Victoria , Australia', -37.4713, 144.7852);
                }
            });
        }
        if (destination != "") {
            geocoder.geocode({ 'address': destination }, function (results, status) {

                if (status == google.maps.GeocoderStatus.OK) {
                    latitude1 = results[0].geometry.location.lat();
                    longitude1 = results[0].geometry.location.lng();
                    destinationdetail = [destination, latitude1, longitude1];
                    locations.push(destinationdetail);
                    GetLocationOnMap(latitude1, longitude1);
                }
                else {
                    $("#errorMsgRegionMap").html(closeButton + "Invalid Destination Address.");
                    $("#errorMsgRegionMap").show();
                    //$("#errorMsgRegionMap").fadeOut(5000);
                    createMarker('Victoria , Australia', -37.4713, 144.7852);
                }

            });

        }
    }

    else {
        $("#errorMsgRegionMap").html(closeButton + "Source or Destination address  are  required");
        $("#errorMsgRegionMap").show();
        //$("#errorMsgRegionMap").fadeOut(5000);

    }

}
function GetLocationOnMap(latitude, longitude) {

    var bounds = new google.maps.LatLngBounds();
    var infowindow = new google.maps.InfoWindow();
    var lat = latitude;
    var lng = longitude;
    var map = new google.maps.Map(document.getElementById('dvMap'), {
        zoom: 10,
        center: new google.maps.LatLng(lat, lng),
        mapTypeId: google.maps.MapTypeId.DRIVING
    });

    var marker, i;
    for (i = 0; i < locations.length; i++) {
        marker = new google.maps.Marker({
            position: new google.maps.LatLng(locations[i][1], locations[i][2]),
            draggable: false,
            map: map
        });

        //extend the bounds to include each marker's position
        bounds.extend(marker.position);

        google.maps.event.addListener(marker, 'click', (function (marker, i) {
            return function () {
                infowindow.setContent(locations[i][0]);
                infowindow.open(map, marker);
            }
        })(marker, i));
    }

    //now fit the map to the newly inclusive bounds
    map.fitBounds(bounds);
}
function geocodeAddress(address) {

    $("#errorMsgRegionMap").hide();
    //var India = new google.maps.LatLng(51.508742, -0.120850);
    //var mapOptions = {
    //    zoom: 11,
    //    center: India
    //};

    //map = new google.maps.Map(document.getElementById('dvMap'),mapOptions);
    var geocoder = new google.maps.Geocoder();
    if (address.trim() == "") {

        createMarker('Victoria , Australia', -37.4713, 144.7852);
    }
    else {
        geocoder.geocode({ address: address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                var p = results[0].geometry.location;
                var lat = p.lat();
                var lng = p.lng();
                createMarker(address, lat, lng);
            }
            else {
                $("#errorMsgRegionMap").html(closeButton + "Invalid Destination Address.");
                $("#errorMsgRegionMap").show();
                //$("#errorMsgRegionMap").fadeOut(5000);
                createMarker('Victoria , Australia', -37.4713, 144.7852);
            }

        }
    );
    }
}
function createMarker(add, lat, lng) {

    //var delay = 1;
    var infowindow = new google.maps.InfoWindow();
    var latlng = new google.maps.LatLng(lat, lng);

    var map = new google.maps.Map(document.getElementById('dvMap'), {
        zoom: 12,
        center: new google.maps.LatLng(lat, lng),
        mapTypeId: google.maps.MapTypeId.DRIVING
    });
    var bounds = new google.maps.LatLngBounds();

    var contentString = add;
    var marker = new google.maps.Marker({
        position: new google.maps.LatLng(lat, lng),
        center: latlng,
        map: map,
        draggable: false
    });

    google.maps.event.addListener(marker, 'click', function () {
        infowindow.setContent(contentString);
        infowindow.open(map, marker);
    });

    // bounds.extend(marker.position);

}
function isValidPhone(event, obj) {
    if (!event.ctrlKey) {
        if (event.which == 43 && $("#" + obj.id).val().length < 2) {
            if ($("#" + obj.id).val().indexOf('+') == -1) {
                return true;
            }
        }
        if (event.which > 31 && (event.which < 48 || event.which > 57)) {
            return false;
        }
        return true;
    }
    return true;
}


function fillJobType() {
    if (BasicDetails_JobID == 0) {
        $("#BasicDetails_JobType").val('1').change();
    }
}



$(document).ready(function () {
    UploadAllSignatureOfJob($("#uploadJobInstallerSignSWH"));
    UploadAllSignatureOfJob($("#uploadJobInstallerSign"));
    UploadAllSignatureOfJob($("#uploadJobDesignerSign"));
    UploadAllSignatureOfJob($("#uploadJobElectricianSign"));
    UploadAllSignatureOfJob($("#uploadJobOwnerSign"));

    if (installerSignSRC != null && installerSignSRC != '') {
        $("#imgInstallerSignatureViewSWH").show();
        $("#imgInstallerSignatureView").show();
    }
    if (designerSignSRC != null && designerSignSRC != '') {
        $("#imgDesignerSignatureView").show();
    }
    if (electricianSignSRC != null && electricianSignSRC != '') {
        $("#imgElectricianSignatureView").show();
    }
    
    $("#BasicDetails_InstallerID").change(function () {
        GetInstallerDesignerElectricianSignature(2, $(this).val());
    });

    $("#BasicDetails_DesignerID").change(function () {
        GetInstallerDesignerElectricianSignature(4, $(this).val());
    });

    $("#imgInstallerSignatureViewSWH").click(function () {
        SRCSignAll = signatureURL + installerSignSRC;
        LoadImageSignature();
    });
    $("#imgInstallerSignatureView").click(function () {    
        SRCSignAll = signatureURL + installerSignSRC;
        LoadImageSignature();
    });
    $("#imgDesignerSignatureView").click(function () {
        SRCSignAll = signatureURL + designerSignSRC;
        LoadImageSignature();
    });
    $("#imgElectricianSignatureView").click(function () {
        SRCSignAll = signatureURL + electricianSignSRC;
        LoadImageSignature();
    });

    $("#JobInstallationDetails_PropertyType").change(function () {
        if (IsGSTSetByAdminUser != 2 && IsRegisteredWithGST == 'True' && ($(this).val() == "Commercial" || $(this).val() == "School"))
            $(".isGSTRegistered").show();
        else {
            $("#BasicDetails_IsGst").prop('checked', false);
            $("#BasicDetails_GSTDocument").val('');
            $("#tblDocuments").find('tr').each(function () {
                $(this).remove();
            });
            $(".isGSTRegistered").hide();
        }
    });

    if (IsGSTSetByAdminUser != 2 && IsRegisteredWithGST == 'True' && ($("#JobInstallationDetails_PropertyType").val() == "Commercial" || $("#JobInstallationDetails_PropertyType").val() == "School")) {
        $(".isGSTRegistered").show();
    }
    else {
        $(".isGSTRegistered").hide();
    }

    if (STCInvoiceCount > 0) {
        $("#BasicDetails_IsGst").prop("disabled", true);
    }
    else {
        $("#BasicDetails_IsGst").prop("disabled", false);
    }

    GSTFileUpload();
    $("#btnClosepopupProof").click(function () {
        $('#popupProof').modal('hide');
    });

    var SoldByDate = $('#BasicDetails_strSoldByDate').val();
    if (SoldByDate != null && SoldByDate != undefined && SoldByDate != '') {
        $('#BasicDetails_strSoldByDate').val('').removeAttr('value');
        $('#BasicDetails_strSoldByDate').datepicker({
            format: GetDateFormat,
            autoclose: true
        }).on('changeDate', function () {
            $(this).datepicker('hide');
        });
        var SoldByDateEdit = moment(SoldByDate).format(GetDateFormat.toUpperCase());
        $('#BasicDetails_strSoldByDate').val(SoldByDateEdit);
    } else {
        $('#BasicDetails_strSoldByDate').datepicker({
            format: GetDateFormat,
            autoclose: true
        }).on('changeDate', function () {
            $(this).datepicker('hide');
        });
    }
    jsonvarSoldByDate = $('#BasicDetails_SoldByDate').val();

    jsonvarJobSoldBy = $('#BasicDetails_SoldBy').val();


    $("#loading-image").show();
    $('#btnPopupSerialNumber').click(function () {
        $('#serialNumberPopup').html('');
        var SerialNumberText = $("#JobSystemDetails_SerialNumbers").val().trim();
        var data = JSON.stringify({ "serialNumbers": $("#JobSystemDetails_SerialNumbers").val(), "jobID": $("#BasicDetails_JobID").val() });
        $.ajax({
            url: urlCheckDuplicateSerialNumbers,
            type: "post",
            async: true,
            dataType: "json",
            data: data,
            cache: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.length > 0) {
                    MasterSerialArray = [];
                    JobSerialArray = [];
                    var SetialArray = $.parseJSON(data);
                    $.grep(SetialArray, function (e, i) {
                        if (!e.IsExistInMaster) { MasterSerialArray.push(e.ExistsSerialNumber); }
                        else { JobSerialArray.push(e.ExistsSerialNumber); }
                    });

                    var lines = SerialNumberText.split("\n");
                    lines = lines.filter(function (n) { return n.length > 0 });
                    for (var lineCount = 1; lineCount < lines.length + 1; lineCount++) {
                        if (JobSerialArray.indexOf(lines[lineCount - 1]) !== -1) {
                            $('#serialNumberPopup').append("<li><span style='background-color: rgba(30, 121, 204, 0.21);'>" + lineCount + ' :' + lines[lineCount - 1] + "</span></li>");
                        }
                        else if (MasterSerialArray.indexOf(lines[lineCount - 1]) !== -1) {
                            $('#serialNumberPopup').append("<li><span style='background-color: rgba(220, 82, 109, 0.21);'>" + lineCount + ' :' + lines[lineCount - 1] + "</span></li>");
                        }
                        else {
                            $('#serialNumberPopup').append("<li><span>" + lineCount + ' :' + lines[lineCount - 1] + "</span></li>");
                        }
                    }

                } else {
                    MasterSerialArray = [];
                    JobSerialArray = [];
                }
            },
        });
        $('#popupSerialNumber').modal({ backdrop: 'static', keyboard: false });
    });


    $("#JobOwnerDetails_UnitTypeID").change(function () {
        if ($('#JobOwnerDetails_UnitTypeID option:selected').val() == "") {
            $('#lblOwnerUnitNumber').removeClass("required");
            $('#lblOwnerUnitTypeID').removeClass("required");
            $('#lblOwnerStreetNumber').addClass("required");
        }
        else {
            $('#lblOwnerUnitNumber').addClass("required");
            $('#lblOwnerUnitTypeID').addClass("required");
            $('#lblOwnerStreetNumber').removeClass("required");
        }
    });


    $("#JobInstallationDetails_UnitTypeID").change(function () {
        if ($('#JobInstallationDetails_UnitTypeID option:selected').val() == "") {
            $('#lblInstallationUnitNumber').removeClass("required");
            $('#lblInstallationUnitTypeID').removeClass("required");
            $('#lblInstallationStreetNumber').addClass("required");
        }
        else {
            $('#lblInstallationUnitNumber').addClass("required");
            $('#lblInstallationUnitTypeID').addClass("required");
            //$('#lblInstallationStreetNumber').removeClass("required");
        }
    });

    $("#JobElectricians_UnitTypeID").change(function () {
        if ($('#JobElectricians_UnitTypeID option:selected').val() == "") {
            $('#lblElectriciansUnitNumber').removeClass("required");
            $('#lblElectriciansUnitTypeID').removeClass("required");
            $('#lblElectriciansStreetNumber').addClass("required");
        }
        else {
            $('#lblElectriciansUnitNumber').addClass("required");
            $('#lblElectriciansUnitTypeID').addClass("required");
            $('#lblElectriciansStreetNumber').removeClass("required");
        }
    });


    $("#JobInstallerDetails_UnitTypeID").change(function () {
        if ($('#JobInstallerDetails_UnitTypeID option:selected').val() == "") {
            $('#lblInstallerUnitNumber').removeClass("required");
            $('#lblInstallerUnitTypeID').removeClass("required");
            $('#lblInstallerStreetNumber').addClass("required");
        }
        else {
            $('#lblInstallerUnitNumber').addClass("required");
            $('#lblInstallerUnitTypeID').addClass("required");
            $('#lblInstallerStreetNumber').removeClass("required");
        }
    });
    $("#JobOwnerDetails_OwnerType").change(function (e) {
        if (this.value == 'Government body' || this.value == 'Corporate body' || this.value == 'Trustee') {
            //add company mendatory
            $("#OwnerCompanyName").addClass("required");
            if ($("#JobOwnerDetails_CompanyName").val() != '') {
                //$("#JobOwnerDetails_CompanyName").addClass('input-validation-error');
            } else {
                $("#JobOwnerDetails_CompanyName").removeClass('valid');
                $("#OwnerCompanyNameValidate").addClass("field-validation-error");
            }
            $("#OwnerCompanyNameValidate").show();
            $("#JobOwnerDetails_CompanyName").rules("add", {
                required: true,
                messages: {
                    required: "Company Name is required."
                }
            });

            //add firstname mendatory
            //$("#JobOwnerDetails_FirstName").rules('remove');
            //$("#JobOwnerDetails_FirstName").removeClass('input-validation-error');
            //$("#OwnerFirstName").removeClass("required");
            //$("#OwnerFirstNameValidate").hide();

            ////add lastname mendatory
            //$("#JobOwnerDetails_LastName").rules('remove');
            //$("#JobOwnerDetails_LastName").removeClass('input-validation-error');
            //$("#OwnerLastName").removeClass("required");
            //$("#OwnerLastNameValidate").hide();

        } else {
            //remove company mendatory
            $("#JobOwnerDetails_CompanyName").rules('remove');
            $("#JobOwnerDetails_CompanyName").removeClass('input-validation-error');
            $("#OwnerCompanyName").removeClass("required");
            $("#OwnerCompanyNameValidate").hide();

            //remove firstname mendatory
            $("#OwnerFirstName").addClass("required");
            if ($("#JobOwnerDetails_FirstName").val() != '') {
                //$("#JobOwnerDetails_FirstName").addClass('input-validation-error');
            } else {
                $("#JobOwnerDetails_FirstName").removeClass('valid');
                $("#OwnerFirstNameValidate").addClass("field-validation-error");
            }
            $("#OwnerFirstNameValidate").show();
            $("#JobOwnerDetails_FirstName").rules("add", {
                required: true,
                messages: {
                    required: "First Name is required."
                }
            });

            //remove lastname mendatory
            $("#OwnerLastName").addClass("required");
            if ($("#JobOwnerDetails_LastName").val() != '') {
                //$("#JobOwnerDetails_LastName").addClass('input-validation-error');
            } else {
                $("#JobOwnerDetails_LastName").removeClass('valid');
                $("#OwnerLastNameValidate").addClass("field-validation-error");
            }
            $("#OwnerLastNameValidate").show();
            $("#JobOwnerDetails_LastName").rules("add", {
                required: true,
                messages: {
                    required: "Last Name is required."
                }
            });
        }
    });
    //divya

    $('#BasicDetails_JobSoldBy').change(function () {
        var jobsoldBy = $('#BasicDetails_JobSoldBy').val();
        $('#BasicDetails_JobSoldByText').val(jobsoldBy);

    });

    $('#btnSoldBy').click(function () {
        $('#popupSoldBy').modal({ backdrop: 'static', keyboard: false });
        if (jsonvarSoldByDate != '') {
            $('#BasicDetails_strSoldByDate').val(jsonvarSoldByDate);
        }
        $('#BasicDetails_JobSoldByText').val(jsonvarJobSoldBy);



    })
    $("#edit").click(function (e) {
        window.location.href = '/Job/Edit/' + jobIDForScheduling;
    });
    $("#JobInstallationDetails_InstallingNewPanel").change(function (e) {
        if ($("#JobInstallationDetails_InstallingNewPanel").val() != "New" && $("#JobInstallationDetails_InstallingNewPanel").val() != "") {
            $("#installationLocation").show();
        } else {
            $("#installationLocation").hide()
        }
    });

    $("#JobSTCDetails_MultipleSGUAddress").change(function (e) {
        if (JOBType == '1') {
            if (this.value == 'Yes') {
                $("#STCLocation").show();
            } else { $("#STCLocation").hide(); }
        }
    });

    $("#btnMainCancel").click(function (e) {
        window.location.href = "/Job/Index";
    });
    setTimeout(function () {
        $("#BasicDetails_JobType").focus();
    }, 3000);


    var jobID = BasicDetails_JobID;
    $.ajax({
        url: urlGetSSCUserInEdit + jobID,
        type: "get",
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.length > 0) {
                $("#SSCID").empty();
                $("#SSCID").append('<option value="">' + "Select" + '</option>');
                $.each(data, function (i, role) {
                    $("#SSCID").append('<option value="' + role.Value + '">' + role.Text + '</option>');
                });
                //$('#SSCID').prop('disabled',true);
            }
            else {
                $("#SSCID").empty();
                $("#SSCID").append('<option value="">' + "Select" + '</option>');
                //$('#SSCID').prop("disabled",false);
            }
        },
    });

    $.ajax({
        url: urlGetSSCUserInDropdown + jobID,
        type: "get",
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.length > 0) {
                $('#SSCID').prop("disabled", true);
                $('#SSCID').val(parseInt(data[0].SSCID));
                $('#btnJobSSCMapping').hide();
            }
            else {
                $('#SSCID').prop("disabled", false);
                $('#btnJobSSCMapping').show();
            }
        },
    });

    if (BasicDetails_ScoID > 0) {
        FillDropDown('ScoID', urlGetSCOUser, BasicDetails_ScoID, true, null);
    }
    else {
        FillDropDown('ScoID', urlGetSCOUser, null, true, null);

    }
    //InstalltionDetail


    var InstallationpostalAddressID = JobInstallationDetails_PostalAddressID || 0;
    var InstallationUnitTypeID = JobInstallationDetails_UnitTypeID || 0;
    var InstallationStreetTypeID = StreetTypeID || 0;


    //FillDropDown('JobInstallationDetails_UnitTypeID', urlGetUnitType, InstallationUnitTypeID, true, fillInstallationAddress);
    //FillDropDown('JobInstallationDetails_StreetTypeID', urlGetStreetType, InstallationStreetTypeID, true, fillInstallationAddress);
    //FillDropDown('JobInstallationDetails_PostalAddressID', urlGetPostalAddress, InstallationpostalAddressID, true, fillInstallationAddress);

    //JobOwnerDetail



    var dropDownData = [];
    dropDownData.push({ id: 'JobInstallationDetails_UnitTypeID', key: "UnitType", value: InstallationUnitTypeID, hasSelect: true, callback: fillInstallationAddress, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
                      { id: 'JobInstallationDetails_StreetTypeID', key: "StreetType", value: InstallationStreetTypeID, hasSelect: true, callback: fillInstallationAddress, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
                      { id: 'JobInstallationDetails_PostalAddressID', key: "PostalAddress", value: InstallationpostalAddressID, hasSelect: true, callback: fillInstallationAddress, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' });


    dropDownData.bindDropdown();

    var OwnerpostalAddressID = JobOwnerDetails_PostalAddressID || 0,
        OwnerUnitTypeID = JobOwnerDetails_UnitTypeID || 0,
        OwnerStreetTypeID = JobOwnerDetails_StreetTypeID || 0;

    //FillDropDown('JobOwnerDetails_UnitTypeID', urlGetUnitType, OwnerUnitTypeID, true, fillOwnerAddress);
    //FillDropDown('JobOwnerDetails_StreetTypeID', urlGetStreetType, OwnerStreetTypeID, true, fillOwnerAddress);
    //FillDropDown('JobOwnerDetails_PostalAddressID', urlGetPostalAddress, OwnerpostalAddressID, true, fillOwnerAddress);
    //JobInstallerrDetail

    var dropDownData = [];
    dropDownData.push({ id: 'JobOwnerDetails_UnitTypeID', key: "UnitType", value: OwnerUnitTypeID, hasSelect: true, callback: fillOwnerAddress, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
                      { id: 'JobOwnerDetails_StreetTypeID', key: "StreetType", value: OwnerStreetTypeID, hasSelect: true, callback: fillOwnerAddress, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
                      { id: 'JobOwnerDetails_PostalAddressID', key: "PostalAddress", value: OwnerpostalAddressID, hasSelect: true, callback: fillOwnerAddress, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' });

    dropDownData.bindDropdown();
    var InstallerpostalAddressID = JobInstallerDetails_PostalAddressID || 0;
    var InstallerUnitTypeID = JobOwnerDetails_UnitTypeID || 0;
    var InstallerStreetTypeID = JobInstallerDetails_StreetTypeID || 0;

    //FillDropDown('JobInstallerDetails_UnitTypeID', urlGetUnitType, InstallerUnitTypeID, true, fillInstallerAddress);
    //FillDropDown('JobInstallerDetails_StreetTypeID', urlGetStreetType, InstallerStreetTypeID, true, fillInstallerAddress);
    //FillDropDown('JobInstallerDetails_PostalAddressID', urlGetPostalAddress, InstallerpostalAddressID, true, fillInstallerAddress);

    var dropDownData = [];
    dropDownData.push({ id: 'JobInstallerDetails_UnitTypeID', key: "UnitType", value: InstallerUnitTypeID, hasSelect: true, callback: fillInstallerAddress, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
                      { id: 'JobInstallerDetails_StreetTypeID', key: "StreetType", value: InstallerStreetTypeID, hasSelect: true, callback: fillInstallerAddress, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
                      { id: 'JobInstallerDetails_PostalAddressID', key: "PostalAddress", value: InstallerpostalAddressID, hasSelect: true, callback: fillInstallerAddress, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' });
    dropDownData.bindDropdown();
    //BasicDetails
    var insatallerid = InstallerID || 0;
    var designerid = DesignerID || 0;
    var distributorid = JobInstallationDetails_DistributorID || 0;



    FillDropDownInstaller('BasicDetails_InstallerID', urlGetInstallerDesignerWithStatus + '?isInstaller=true&existUserId=' + insatallerid + '&solarCompanyId=' + $("#BasicDetails_SolarCompanyId").val() + '&jobId=' + BasicDetails_JobID, insatallerid, true, null);
    FillDropDownInstaller('BasicDetails_DesignerID', urlGetInstallerDesignerWithStatus + '?isInstaller=false&existUserId=' + designerid + '&solarCompanyId=' + $("#BasicDetails_SolarCompanyId").val() + '&jobId=' + BasicDetails_JobID, designerid, true, null);
    
    if (JOBType == '1' || JOBType == '0')
        FillDropDown('JobElectricians_ElectricianID', urlGetElectrician + $("#BasicDetails_JobID").val() + '&solarCompanyId=' + $("#BasicDetails_SolarCompanyId").val(), JobElectricians_ElectricianID, true, null);
    else {    
        FillDropDown('JobInstallerDetails_ElectricianID', urlGetElectrician + $("#BasicDetails_JobID").val() + '&solarCompanyId=' + $("#BasicDetails_SolarCompanyId").val() + '&JobType=' + $("#BasicDetails_JobType").val(), JobInstallerDetails_ElectricianID, true, null);
        SWHInstallerSignatureUpload();        
    }

    FillDropDown('BasicDetails_JobSoldBy', urlGetSoldBy, 0, true, null);
    FillDropDown('JobInstallationDetails_DistributorID', urlGetDestributors, distributorid, true, null);

    var jobtype, jobstage, priority, deemingperiod;
    jobtype = JOBType || 0;
    jobstage = BasicDetails_JobStage || 0;
    priority = BasicDetails_Priority || 0;
    deemingperiod = JobSTCDetails_DeemingPeriod || 0;

    FillDropDown('BasicDetails_JobStage', urlGetJobStage, jobstage, true, fillJobStage);
    FillDropDown('BasicDetails_Priority', urlGetPriority, priority, true, null);

    var Installationid = JobInstallationDetails_ElectricityProviderID || 0;

    //InstallationDetails
    FillDropDown('JobInstallationDetails_ElectricityProviderID', urlGetElectricityProvider, Installationid, true, null);

    var installationDate = $("#BasicDetails_strInstallationDate").val();
    var installationDateEdit = '';
    var finalYear = '';

    var tempInstallationDate = $("#BasicDetails_strInstallationDateTemp").val();

    //if (installationDate != null && installationDate != undefined && installationDate != '') {
    //    finalYear = moment(installationDate).format('yyyy'.toUpperCase());
    //}

    //alert(tempInstallationDate);

    if (tempInstallationDate != null && tempInstallationDate != undefined && tempInstallationDate != '') {
        finalYear = moment(tempInstallationDate).format('yyyy'.toUpperCase());
    }

    //STC Details
    FillDropDown('JobSTCDetails_DeemingPeriod', urlGetDeemingPeriod + finalYear, deemingperiod, true, null);

    $("#BasicDetails_strInstallationDate").change(function () {
        var changeFinalYear = '', changeDeemingPeriod;
        var changeinstallationDate = $("#BasicDetails_strInstallationDate").val();
        if (changeinstallationDate != null && changeinstallationDate != undefined && changeinstallationDate != '') {
            changeFinalYear = moment(changeinstallationDate, "DD/MM/YYYY").format('yyyy'.toUpperCase());
        }
        changeDeemingPeriod = $('#JobSTCDetails_DeemingPeriod').val();
        FillDropDown('JobSTCDetails_DeemingPeriod', urlGetDeemingPeriod + changeFinalYear, changeDeemingPeriod, true, null);
    });

    //JobElectricians
    var ElectricianspostalAddressID = JobElectricians_PostalAddressID || 0;

    var ElectriciansUnitTypeID = JobElectricians_UnitTypeID || 0;
    var ElectriciansStreetTypeID = JobElectricians_StreetTypeID || 0;

    //FillDropDown('JobElectricians_UnitTypeID', urlGetUnitType, ElectriciansUnitTypeID, true, fillBasicAddress);
    //FillDropDown('JobElectricians_StreetTypeID', urlGetStreetType, ElectriciansStreetTypeID, true, fillBasicAddress);
    //FillDropDown('JobElectricians_PostalAddressID', urlGetPostalAddress, ElectricianspostalAddressID, true, fillBasicAddress);


    var dropDownData = [];
    dropDownData.push({ id: 'JobElectricians_UnitTypeID', key: "UnitType", value: ElectriciansUnitTypeID, hasSelect: true, callback: fillBasicAddress, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
                      { id: 'JobElectricians_StreetTypeID', key: "StreetType", value: ElectriciansStreetTypeID, hasSelect: true, callback: fillBasicAddress, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
                      { id: 'JobElectricians_PostalAddressID', key: "PostalAddress", value: ElectricianspostalAddressID, hasSelect: true, callback: fillBasicAddress, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' });
    dropDownData.bindDropdown();



    FillDropDownInstaller('JobElectricians_InstallerID', urlGetInstallerDesignerWithStatus + '?isInstaller=true&existUserId=' + insatallerid + '&solarCompanyId=' + $("#BasicDetails_SolarCompanyId").val() + '&jobId=' + BasicDetails_JobID, JobElectricians_InstallerID, true, null);

    var JobPanelDetailsBrand = JobPanelDetails_Brand || '0';
    //JobPanelDetails

    FillDropDown('JobPanelDetails_Brand', urlGetPanelBrand, JobPanelDetailsBrand, true, null);

    $("#btnCalculatedSTC,#btnCalculatedSTCSWH").click(function (e) {
        CalculateStc1();
    })

    $("#JobPanelDetails_Brand").change(function (e) {

        if (this.selectedIndex > 0) {
            $("#JobPanelDetails_Model").prop("disabled", false);
            FillDropDown('JobPanelDetails_Model', urlGetPanelModel + $("#JobPanelDetails_Brand").val() + '&JobType=1', 0, true, null);
        } else {
            $("#JobPanelDetails_Model").prop("disabled", true);
            $("#JobPanelDetails_NoOfPanel").prop("disabled", true);
        }
    });

    $("#JobPanelDetails_Model").change(function (e) {
        if (this.selectedIndex > 0) {
            $("#JobPanelDetails_NoOfPanel").prop("disabled", false);
        } else {
            $("#JobPanelDetails_NoOfPanel").prop("disabled", true);
            $("#JobPanelDetails_NoOfPanel").val("");
        }
    });

    //JobInverterDetails
    var JobInverterDetailsBrand = JobInverterDetails_Brand || 0;
    FillDropDown('JobInverterDetails_Brand', urlGetInverterBrand, JobInverterDetailsBrand, true, null);


    $("#JobInverterDetails_Brand").change(function () {
        if (this.selectedIndex > 0) {
            $("#JobInverterDetails_Series").prop("disabled", false);
            FillDropDown('JobInverterDetails_Series', urlGetInverterModel + $("#JobInverterDetails_Brand").val(), 0, true, null);
        } else {
            $("#JobInverterDetails_Model").prop("disabled", true);
            $("#JobInverterDetails_Series").prop("disabled", true);
        }
    });

    $("#JobInverterDetails_Series").change(function () {
        if (this.selectedIndex > 0) {
            $("#JobInverterDetails_Model").prop("disabled", false);
            FillDropDown('JobInverterDetails_Model', urlGetInverterSeries + $("#JobInverterDetails_Series").val() + '&Manufacturer=' + $("#JobInverterDetails_Brand").val(), 0, true, null);
        } else {
            $("#JobInverterDetails_Model").prop("disabled", true);
        }
    });

    $("#JobPanelDetails_Brand").change(function () {
        if (this.selectedIndex > 0) {
            $("#JobPanelDetails_Model").prop("disabled", false);
            FillDropDown('JobPanelDetails_Model', urlGetPanelModel + $("#JobPanelDetails_Brand").val() + '&JobType=1', 0, true, null);
        } else {
            $("#JobPanelDetails_Model").prop("disabled", true);
        }
    });

    //battery
    //var JobBatteryManufacturer = JobBatteryManufacturer_Manufacturer || 0;
    FillDropDown('JobBatteryManufacturer_Manufacturer', urlGetBatteryManufacturer, null, true, null);

    $("#JobBatteryManufacturer_Manufacturer").change(function (e) {

        if (this.selectedIndex > 0) {
            $("#JobBatteryManufacturer_ModelNumber").prop("disabled", false);
            FillDropDown('JobBatteryManufacturer_ModelNumber', urlGetBatteryModel + $("#JobBatteryManufacturer_Manufacturer").val(), 0, true, null);
        } else {
            $("#JobBatteryManufacturer_ModelNumber").prop("disabled", true);            
        }
    });

    //JobSystemDetails
    var JobSystemDetailsSystemBrand = JobSystemDetails_SystemBrand || 0;

    FillDropDown('JobSystemDetails_SystemBrand', urlGetPanelBrand2, JobSystemDetailsSystemBrand, true, null);


    $("#JobSystemDetails_SystemBrand").change(function () {
        if (this.selectedIndex > 0) {
            $("#JobSystemDetails_SystemModel").prop("disabled", false);
            FillDropDown('JobSystemDetails_SystemModel', urlGetPanelModel + $("#JobSystemDetails_SystemBrand").val() + '&JobType=2', 0, true, null);
        } else {
            $("#JobSystemDetails_SystemModel").prop("disabled", true);
        }
    });

    var latitude;
    var longitude;
    var latitude1;
    var longitude1;
    var locations = [];
    var sourcedetail = [];
    var destinationdetail = [];
    var USERID = Model_Guid;

    var maxsize = Math.round(MaxImageSize_Job / 1024000);

    $("#JobSystemDetails_NoOfPanel").attr("data-val", "false");

    if ($("#BasicDetails_JobID").val() == "0") {
        $("#JobPanelDetails_NoOfPanel").val("");
        $("#JobInstallationDetails_ExistingSystemSize").val("");
        $("#JobInstallationDetails_NoOfPanels").val("");
        $("#JobSystemDetails_SystemSize").val("");
        $("#JobSystemDetails_CalculatedSTC").val("");
        $("#JobSystemDetails_CalculatedSTCForSWH").val("");
        $("#JobSystemDetails_NoOfPanel").val("");
        $("#JobPanelDetails_NoOfPanel").prop("disabled", true);
        $("#deletejob").hide();
        $("#ExistingSystem").hide();
    } else {
        if ($("#JobPanelDetails_NoOfPanel").val() == "0") { $("#JobPanelDetails_NoOfPanel").val(""); }
        $("#JobPanelDetails_NoOfPanel").prop("disabled", true);
        if ($("#JobInstallationDetails_ExistingSystemSize").val() == "0.00") { $("#JobInstallationDetails_ExistingSystemSize").val(""); }
        if ($("#JobInstallationDetails_NoOfPanels").val() == "0") { $("#JobInstallationDetails_NoOfPanels").val(""); }
        if ($("#JobSystemDetails_CalculatedSTC").val() == "0.00") { $("#JobSystemDetails_CalculatedSTC").val(""); }
        if ($("#JobSystemDetails_CalculatedSTCForSWH").val() == "0.00") { $("#JobSystemDetails_CalculatedSTCForSWH").val(""); }
        if ($("#JobSystemDetails_NoOfPanel").val() == "0") { $("#JobSystemDetails_NoOfPanel").val(""); }
        if ($("#JobSystemDetails_SystemSize").val() == "0.00") { $("#JobSystemDetails_SystemSize").val(""); }

        //var installationDate = $("#BasicDetails_strInstallationDate").val();
        //if (installationDate != null && installationDate != undefined && installationDate != '') {
        //    $("#BasicDetails_strInstallationDate").val('').removeAttr('value');
        //    $('.date-pick, .date-pick1, .date-pick').datepicker({
        //        format: GetDateFormat,
        //        autoclose: true
        //    }).on('changeDate', function () {
        //        $(this).datepicker('hide');
        //    });
        //    var installationDateEdit = moment(installationDate).format(GetDateFormat.toUpperCase());
        //    $("#BasicDetails_strInstallationDate").val(installationDateEdit);
        //} else {
        //    $('.date-pick, .date-pick1, .date-pick').datepicker({
        //        format: GetDateFormat,
        //        autoclose: true
        //    }).on('changeDate', function () {
        //        $(this).datepicker('hide');
        //    });
        //}
        if (JOBType == '1') {
            if (JobSTCDetails_MultipleSGUAddress == 'Yes') {
                $("#STCLocation").show();
            } else {
                $("#STCLocation").hide();
            }
        }

        $("#InvoiceStartDate").val('').removeAttr('value');

        if (JobInstallationDetails_InstallingNewPanel == 'Replacing' || JobInstallationDetails_InstallingNewPanel == 'Adding' || JobInstallationDetails_InstallingNewPanel == 'Extension') {
            $("#installationLocation").show();
        }

        if (USERType == 4 || (USERType == 8 && IsUnderSSC == "False")) {
            $("#deletejob").show();
        }
        else {
            $("#deletejob").hide();
        }
        $("#BasicDetails_JobType").prop("disabled", true);
        if (JobElectricians_Signature) {

            $('#imgSign').attr('src', JobElectricians_Signature)
            var SignName = $('#imgSign').attr('src');

            var guid = Model_Guid;
            var proofDocumentURL = UploadedDocumentPath;
            var imagePath = proofDocumentURL + "JobDocuments" + "/" + guid;
            var SRC = imagePath + "/" + SignName;

            SRCSign = SRC;

            //$('#imgSign').attr('src', SRC).load(function () { logoWidth = this.width; logoHeight = this.height });;
            $('#imgSign').attr('class', SignName);

            $('#imgSignature').show();
        }

        if (SRCOwnerSign != null && SRCOwnerSign != '') {
            SRCOwnerSign = signatureURL + SRCOwnerSign;
            $('#imgOwnerSignatureView').show();
        }
        $('#imgOwnerSignatureView').click(function () {
            $("#loading-image").css("display", "");
            $('#imgOwnerSign').attr('src', SRCOwnerSign).load(function () {
                logoWidth1 = this.width;
                logoHeight1 = this.height;
                $('#popupOwnerSign').modal({ backdrop: 'static', keyboard: false });

                if ($(window).height() <= logoHeight1) {
                    //$("#imgOwnerSign").height($(window).height() - 150);

                    $("#imgOwnerSign").closest('div').height($(window).height() - 150);
                    $("#imgOwnerSign").closest('div').css('overflow-y', 'scroll');
                    $("#imgOwnerSign").height(logoHeight1);
                }
                else {
                    $("#imgOwnerSign").height(logoHeight1);
                    $("#imgOwnerSign").closest('div').removeAttr('style');
                }

                if (screen.width <= logoWidth1 || logoWidth1 >= screen.width - 250) {
                    //$("#imgOwnerSign").width(screen.width - 10);
                    //$('#popupOwnerSign').find(".modal-dialog").width(screen.width - 10);

                    $('#popupOwnerSign').find(".modal-dialog").width(screen.width - 250);
                    $("#imgOwnerSign").width(logoWidth1);
                }
                else {
                    $("#imgOwnerSign").width(logoWidth1);
                    $('#popupOwnerSign').find(".modal-dialog").width(logoWidth1);
                }
                $("#loading-image").css("display", "none");
            });
            $("#imgOwnerSign").unbind("error");
            $('#imgOwnerSign').attr('src', SRCOwnerSign).error(function () {
                alert('Image does not exist.');
                $("#loading-image").css("display", "none");
            });
        });

        $("#btnClosepopupOwnerSign").click(function () {
            $('#popupOwnerSign').modal('toggle');
        });

        $("#btnClosePopupSign").click(function () {
            $('#popupSign').modal('toggle');
        });

        $('#tblPanel tr').each(function () {
            $("#gridPanel").show();
            var PanelBrand = $(this).find("td").eq(0).attr("class");
            var PanelModel = $(this).find("td").eq(1).attr("class");
            var NoOfPanel = $(this).find("td").eq(2).attr("class");
            var PanelSupplier = $(this).find("td").eq(3).attr("data-supplier");
            var trId = $(this).find("td").eq(3).attr("class");
            PanelXml.push({ ID: trId, Brand: PanelBrand, Model: PanelModel, Count: NoOfPanel, Supplier: PanelSupplier });
        });
        $('#tblInverter tr').each(function () {
            $("#gridInverter").show();
            var InverterBrand = $(this).find("td").eq(0).attr("class");
            var series = $(this).find("td").eq(1).attr("class");
            var InverterModel = $(this).find("td").eq(2).attr("class");
            var trId = $(this).find("td").eq(3).attr("class");
            InverterXml.push({ ID: trId, Brand: InverterBrand, Series: series, Model: InverterModel });
        });

        $('#tblBatteryManufacturer tr').each(function () {
            $("#gridBattery").show();
            var batteryManufacturer = $(this).find("td").eq(0).attr("class");
            var batteryModel = $(this).find("td").eq(1).attr("class");
            var trId = $(this).find("td").eq(2).attr("class");
            batteryXml.push({ JobBatteryManufacturerId: trId, Manufacturer: batteryManufacturer, ModelNumber: batteryModel });
        });

        //Address hide-show



        if (JobOwnerDetails_AddressID == 1) {
            $('.OwnerDPA').show();
            $('.OwnerPDA').hide();
        }
        else {
            $('.OwnerPDA').show();
            $('.OwnerDPA').hide();
        }

        if (JobInstallerDetails_AddressID == 1) {
            $('.InstallerDPA').show();
            $('.InstallerPDA').hide();
        }
        else {
            $('.InstallerPDA').show();
            $('.InstallerDPA').hide();
        }

        if (JobInstallationDetails_AddressID == 1) {
            $('.InstallationDPA').show();
            $('.InstallationPDA').hide();
        }
        else {
            $('.InstallationPDA').show();
            $('.InstallationDPA').hide();
        }



        if (JobElectricians_AddressID == 1) {
            $('.ElectriciansDPA').show();
            $('.ElectriciansPDA').hide();
        }
        else {
            $('.ElectriciansPDA').show();
            $('.ElectriciansDPA').hide();
        }

        if (JOBType == '2') {

            $("#TypeOfConnection,#JobSTCDetails_TypeOfConnection,#SystemMountingType,#JobSTCDetails_SystemMountingType").hide();
            $("#DeemingPeriod,#JobSTCDetails_DeemingPeriod,#InstallerID,#BasicDetails_InstallerID,#DesignerID,#BasicDetails_DesignerID").hide();
            //$("#DeemingPeriod,#JobSTCDetails_DeemingPeriod,#InstallerID,#DesignerID,#BasicDetails_DesignerID").hide();
            $("#NMI,#JobInstallationDetails_NMI,#InstallingNewPanel,#InstallingNewPanel,#InstallingNewPanel,#JobInstallationDetails_InstallingNewPanel,#MeterNumber,#JobInstallationDetails_MeterNumber,#PhaseProperty,#JobInstallationDetails_PhaseProperty,#ElectricityProviderID,#JobInstallationDetails_ElectricityProviderID,#ExistingSystem1,#JobInstallationDetails_ExistingSystem,#ExistingSystem").hide();
            $("#installContent").show();
            $(".SWSystemDetails").show();
            $(".PVDSystemDetail").hide();
            $("#serialNumberLable").html("Tank serial number(s):");
            $("#MultipleSGUAddress").html("Is there more than one SWH/ASHP at this address:");
            $("#CertificateCreated").html("Creating certificates for previously failed SWH:");
            $("#divVolumetricCapacity").show();
            $("#divSecondhandWaterHeater").show();
            $("#JobSystemDetails_NoOfPanel").removeAttr("readonly");
            $("#electricianDetail").hide();
            if (JobSTCDetails_VolumetricCapacity == 'Yes') {
                $("#divStatutoryDeclarations").show();
            }
        } else if (JOBType == '1') {
            $("#TypeOfConnection,#JobSTCDetails_TypeOfConnection,#SystemMountingType,#JobSTCDetails_SystemMountingType").show();
            $("#DeemingPeriod,#JobSTCDetails_DeemingPeriod,#InstallerID,#BasicDetails_InstallerID,#DesignerID,#BasicDetails_DesignerID").show();
            $("#NMI,#JobInstallationDetails_NMI,#InstallingNewPanel,#InstallingNewPanel,#InstallingNewPanel,#JobInstallationDetails_InstallingNewPanel,#MeterNumber,#JobInstallationDetails_MeterNumber,#PhaseProperty,#JobInstallationDetails_PhaseProperty,#ElectricityProviderID,#JobInstallationDetails_ElectricityProviderID,#ExistingSystem1,#JobInstallationDetails_ExistingSystem").show();
            $("#installContent").hide();
            $("#serialNumberLable").html("Equipment model serial number(s):");
            $("#MultipleSGUAddress").html("Is there more than one SGU at this address?:");
            $("#CertificateCreated").html("Are you creating certificates for a system that has previously been failed by the Clean Energy Regulator?:");
            $(".SWSystemDetails").hide();
            $(".PVDSystemDetail").show();
            $("#divVolumetricCapacity").hide();
            $("#divStatutoryDeclarations").hide();
            $("#divSecondhandWaterHeater").hide();
            $("#JobSystemDetails_NoOfPanel").attr("readonly");
            $("#electricianDetail").show();
        }


        //Owner detail

        //var FirstName = $("#JobOwnerDetails_FirstName").val();
        //var LastName = $("#JobOwnerDetails_LastName").val();
        //clientname = FirstName + ' ' + LastName;
        var clientname;
        if ($("#JobOwnerDetails_OwnerType").val() == 'Government body' || $("#JobOwnerDetails_OwnerType").val() == 'Corporate body' || $("#JobOwnerDetails_OwnerType").val() == 'Trustee') {
            clientname = $("#JobOwnerDetails_CompanyName").val();
        } else {
            var FirstName = $("#JobOwnerDetails_FirstName").val();
            var LastName = $("#JobOwnerDetails_LastName").val();
            clientname = FirstName + ' ' + LastName;
        }
        $("#txtClientName").val(clientname);
        $('#spantxtClientNames').hide();

        //Installer Detail
        var Installername;
        var FirstName = $("#JobInstallerDetails_FirstName").val();
        var Surname = $("#JobInstallerDetails_Surname").val();
        Installername = FirstName + ' ' + Surname;
        $("#txtInstallerName").val(Installername);


        //Existing system
        if (JobInstallationDetails_ExistingSystem == 'False') {
            $("#ExistingSystem").hide();
        } else {
            $("#ExistingSystem").show();
        }

        //GST Document
        if (BasicDetails_IsGst == 'False') {
            $("#jobGST").hide();
        }
        else {
            $("#jobGST").show();
        }

        if (JobSTCDetails_CertificateCreated == "No") {
            $("#FailedAccreditationCode").hide();
            $("#JobSTCDetails_FailedAccreditationCode").hide();

            $("#FailedReason").hide();
            $("#JobSTCDetails_FailedReason").hide();
        } else if (JobSTCDetails_CertificateCreated == "Yes") {
            $("#FailedAccreditationCode").show();
            $("#JobSTCDetails_FailedAccreditationCode").show();

            $("#FailedReason").show();
            $("#JobSTCDetails_FailedReason").show();
        }


        $("#JobSystemDetails_SystemModel").prop("disabled", false);
        var JobSystemDetailsSystemModel = JobSystemDetails_SystemModel || 0;
        FillDropDown('JobSystemDetails_SystemModel', urlGetPanelModel + JobSystemDetails_SystemBrand + '&JobType=' + JOBType, JobSystemDetailsSystemModel, true, null);

        if (JOBType == 1) {
            if (USERType == 8 && IsUnderSSC == "True") {
                //Readonly & disable
                $("#JobInstallationDetails_IsSameAsOwnerAddress").attr('disabled', 'disabled');
                $("#JobInstallationDetails_ExistingSystem").attr('disabled', 'disabled');
                $("#JobOwner").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#JobOwner").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });
                $("#JobInstallationAddress").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#JobInstallationAddress").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });

                $("#frmCreateJob").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#frmCreateJob").find('textarea').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#frmCreateJob").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });

                $("#BasicDetails_strInstallationDate").attr('disabled', 'disabled');

                //button hide
                $("#JobInstallationAddress").find('button[type=button]').each(function () {

                    $(this).hide();
                });
                $("#JobOwner").find('button[type=button]').each(function () {

                    $(this).hide();
                });

                $("#btnCalculatedSTC").hide();
                $("#btnAddPanel").hide();
                $("#btnAddInverter").hide();
                //

                //for some remove Readonly & disable
                $("#BasicDetails_InstallerID").removeAttr("disabled");
                $("#BasicDetails_DesignerID").removeAttr("disabled");


                $('#JobElectricians_InstallerID').removeAttr("disabled");
                $('#JobElectricians_CompanyName').removeAttr("Readonly");
                $('#JobElectricians_FirstName').removeAttr("Readonly");
                $('#JobElectricians_LastName').removeAttr("Readonly");
                $('#JobElectricians_Email').removeAttr("Readonly");
                $('#JobElectricians_Phone').removeAttr("Readonly");
                $('#JobElectricians_Mobile').removeAttr("Readonly");
                $('#JobElectricians_LicenseNumber').removeAttr("Readonly");
            }

            if (USERType == 7 || USERType == 9) {
                $("#JobInstallationDetails_IsSameAsOwnerAddress").attr('disabled', 'disabled');
                $("#JobInstallationDetails_ExistingSystem").attr('disabled', 'disabled');
                $("#JobElectricians").find('input[type=text]').each(function () {
                    $(this).attr('Readonly', 'true');
                });
                $("#JobElectricians").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });
                $("#JobOwner").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#JobOwner").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });
                $("#JobInstallationAddress").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#JobInstallationAddress").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });

                $("#frmCreateJob").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#frmCreateJob").find('textarea').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#frmCreateJob").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });
                //button hide
                $("#JobElectricians").find('button[type=button]').each(function () {

                    $(this).hide();
                });
                $("#JobInstallationAddress").find('button[type=button]').each(function () {

                    $(this).hide();
                });
                $("#JobOwner").find('button[type=button]').each(function () {

                    $(this).hide();
                });
                $("#divUpload").hide();
                $("#btnCalculatedSTC").hide();
                $("#btnAddPanel").hide();
                $("#btnAddInverter").hide();
                //

                $("#BasicDetails_strInstallationDate").removeAttr("Readonly");
                $('#BasicDetails_JobStage').removeAttr("disabled");

            }
            if (USERType == 2 || USERType == 5) {
                $("#JobInstallationDetails_IsSameAsOwnerAddress").attr('disabled', 'disabled');
                $("#JobInstallationDetails_ExistingSystem").attr('disabled', 'disabled');
                $("#JobElectricians").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#JobElectricians").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });
                $("#JobOwner").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#JobOwner").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });
                $("#JobInstallationAddress").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#JobInstallationAddress").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });

                $("#frmCreateJob").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#frmCreateJob").find('textarea').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#frmCreateJob").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });
                $("#BasicDetails_strInstallationDate").attr('disabled', 'disabled');

                //button hide
                $("#JobElectricians").find('button[type=button]').each(function () {

                    $(this).hide();
                });
                $("#JobInstallationAddress").find('button[type=button]').each(function () {

                    $(this).hide();
                });
                $("#JobOwner").find('button[type=button]').each(function () {

                    $(this).hide();
                });
                $("#divUpload").hide();
                $("#btnCalculatedSTC").hide();
                $("#btnAddPanel").hide();
                $("#btnAddInverter").hide();
                //$("#btnDropMainSave").hide();
                $("#btnMainCancel").hide();
            }
        }

        if (JOBType == 2) {
            if (USERType == 7 || USERType == 9 || (USERType == 8 && IsUnderSSC == "True")) {

                $("#JobInstallationDetails_IsSameAsOwnerAddress").attr('disabled', 'disabled');
                $("#JobInstallationDetails_ExistingSystem").attr('disabled', 'disabled');
                $("#JobElectricians").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#JobElectricians").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });

                $("#JobOwner").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#JobOwner").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });
                $("#JobInstallationAddress").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#JobInstallationAddress").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });

                $("#frmCreateJob").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#frmCreateJob").find('textarea').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#frmCreateJob").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });
                $("#BasicDetails_strInstallationDate").attr('disabled', 'disabled');

                //button hide
                $("#JobElectricians").find('button[type=button]').each(function () {

                    $(this).hide();
                });

                $("#JobInstallationAddress").find('button[type=button]').each(function () {

                    $(this).hide();
                });
                $("#JobOwner").find('button[type=button]').each(function () {

                    $(this).hide();
                });
                $("#divUpload").hide();
                $("#btnCalculatedSTCSWH").hide();
                $("#btnAddPanel").hide();
                $("#btnAddInverter").hide();
                $("#BasicDetails_strInstallationDate").removeAttr("Readonly");
                $('#BasicDetails_JobStage').removeAttr("disabled");
            }
            if (USERType == 2 || USERType == 5) {
                $("#JobInstallationDetails_IsSameAsOwnerAddress").attr('disabled', 'disabled');
                $("#JobInstallationDetails_ExistingSystem").attr('disabled', 'disabled');
                $("#JobElectricians").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#JobElectricians").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });
                $("#JobOwner").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#JobOwner").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });
                $("#JobInstallationAddress").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#JobInstallationAddress").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });
                $("#Installer").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#Installer").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });
                $("#frmCreateJob").find('input[type=text]').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#frmCreateJob").find('textarea').each(function () {

                    $(this).attr('Readonly', 'true');
                });
                $("#frmCreateJob").find('Select').each(function () {

                    $(this).attr('disabled', 'disabled');
                });
                $("#BasicDetails_strInstallationDate").attr('disabled', 'disabled');

                //button hide
                $("#JobElectricians").find('button[type=button]').each(function () {

                    $(this).hide();
                });
                $("#JobInstallationAddress").find('button[type=button]').each(function () {

                    $(this).hide();
                });
                $("#Installer").find('button[type=button]').each(function () {

                    $(this).hide();
                });
                $("#Installer").find('button[type=button]').each(function () {

                    $(this).hide();
                });
                $("#divUpload").hide();
                $("#btnCalculatedSTCSWH").hide();
                $("#btnAddPanel").hide();
                $("#btnAddInverter").hide();
                //$("#btnDropMainSave").hide();
                $("#btnMainCancel").hide();
            }
        }
    }
    $('#imgSignature').click(function () {    
        $("#loading-image").css("display", "");
        $('#imgSign').attr('src', SRCSign).load(function () {
            logoWidth = this.width;
            logoHeight = this.height;
            $('#popupbox').modal({ backdrop: 'static', keyboard: false });

            if ($(window).height() <= logoHeight) {
                //$("#imgSign").height($(window).height() - 205);

                $("#imgSign").closest('div').height($(window).height() - 205);
                $("#imgSign").closest('div').css('overflow-y', 'scroll');
                $("#imgSign").height(logoHeight);
            }
            else {
                $("#imgSign").height(logoHeight);
                $("#imgSign").closest('div').removeAttr('style');
            }

            if (screen.width <= logoWidth || logoWidth >= screen.width - 250) {
                //$("#imgSign").width(screen.width - 10);
                //$('#popupbox').find(".modal-dialog").width(screen.width - 10);

                $('#popupbox').find(".modal-dialog").width(screen.width - 250);
                $("#imgSign").width(logoWidth);
            }
            else {
                $("#imgSign").width(logoWidth);
                $('#popupbox').find(".modal-dialog").width(logoWidth);
            }
            $("#loading-image").css("display", "none");
        });
        $("#imgSign").unbind("error");
        $('#imgSign').attr('src', SRCSign).error(function () {
            alert('Image does not exist.');
            $("#loading-image").css("display", "none");
        });

    });
    $("#btnClosePopUpBox").click(function () {
        $('#popupbox').modal('toggle');
    });
    $("#BasicDetails_JobType").change(function (e) {
        jobChange();
        if (this.value == "1" || this.value == "2") {
            $.ajax({
                url: urlGetJobNumber + this.value,
                type: "GET",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8', // Not to set any content header
                processData: false, // Not to process data
                cache: false,
                async: true,
                success: function (result) {
                    $("#BasicDetails_JobNumber").val(result);
                }
            });
        }
    });

    //$("#JobSTCDetails_InstallingCompleteUnit").change(function(e){
    //    if (this.value == "No") {
    //        $("#AdditionalCapacityNotes").show();
    //        $("#JobSTCDetails_AdditionalCapacityNotes").show();
    //    } else if (this.value == "Yes") {
    //        $("#AdditionalCapacityNotes").hide();
    //        $("#JobSTCDetails_AdditionalCapacityNotes").hide();
    //    }
    //})

    $("#JobSTCDetails_CertificateCreated").change(function (e) {
        $("#spanFailedAccreditationCode").hide();
        $('#JobSTCDetails_FailedAccreditationCode').val('');

        $('#JobSTCDetails_FailedReason').val('');
        if (this.value == "No") {
            $("#FailedAccreditationCode").hide();
            $("#JobSTCDetails_FailedAccreditationCode").hide();

            $("#FailedReason").hide();
            $("#JobSTCDetails_FailedReason").hide();
        } else if (this.value == "Yes") {
            $("#FailedAccreditationCode").show();
            $("#JobSTCDetails_FailedAccreditationCode").show();

            $("#FailedReason").show();
            $("#JobSTCDetails_FailedReason").show();
        }
    });
    //todo
    $("ul#SaveJob").on("click", "li", function () {
        //$("#frmCreateJob").validate().settings.ignore = [];
        fillElectrianPopup();
        fillOwnerPopup();
        fillInstallationPopup();
        if ($('#BasicDetails_JobType').val() == 2) {
            //fillInstallerPopup();
        }
        $(".sidebar-list").show();

        if ($("#JobInstallationDetails_PropertyType").val() == "Commercial" || $("#JobInstallationDetails_PropertyType").val() == "School")
            $(".isGSTRegistered").show();
        else
            $(".isGSTRegistered").hide();

        $('.menuheader').addClass('openheader');
        //$("#BasicDetails_JobType").change();
        jobChange();
        $('#JobInstallationDetails_ExistingSystem').change();
        var serialVal = SerialNumbersValidation();
        var popup = CheckShowErrorMessagesForPopup();
        var validpage = $("#frmCreateJob").valid();



        // Check to set STC value start
        var isRequiredFieldofSTC;
        if ($("#BasicDetails_JobType").val() == 1) //PVD job
            isRequiredFieldofSTC = ($("#BasicDetails_strInstallationDate").val() && $("#JobSTCDetails_DeemingPeriod").val() && $("#JobInstallationDetails_PostCode").val()) ? true : false;
        else //SWH job
            isRequiredFieldofSTC = ($("#JobSystemDetails_SystemBrand").val() && $("#JobSystemDetails_SystemModel").val() && $("#JobInstallationDetails_PostCode").val()) ? true : false;

        var isSystemSize = $("#JobSystemDetails_SystemSize").val() > 0 ? true : false;
        var isInstallationDate = $("#BasicDetails_strInstallationDate").val() ? true : false;

        var isValidDataForSTC;

        if ($("#BasicDetails_JobType").val() == 1)
            isValidDataForSTC = !isSystemSize || (isSystemSize && isRequiredFieldofSTC);
        else
            isValidDataForSTC = !isInstallationDate || (isInstallationDate && isRequiredFieldofSTC);
        // Check to set STC value end



        if (validpage && popup && serialVal) {

            if (isValidDataForSTC) {
                $("#loading-image").show();
                jobSaveType = $(this).index();
                checkBusinessRules($(this).index(), false);
            }
            else {
                if ($("#BasicDetails_JobType").val() == 1)
                    jobErrorMessage("Please fill Installation Date, STC DeemingPeriod, Installation postcode to set STC value.");
                else
                    jobErrorMessage("Please fill Installation Date, System brand, System Model, Installation postcode to set STC value.");
            }

        } else {
            jobErrorMessage("Please fill  all required fields.");
            return;
        }
    });

    Array.prototype.pushArray = function () {
        var toPush = this.concat.apply([], arguments);
        for (var i = 0, len = toPush.length; i < len; ++i) {
            this.push(toPush[i]);
        }
    };
    $.fn.serializeToJson = function () {
        var $form = $(this[0]);
        var $installation = $("#addressPopup");
        var $electrician = $("#JobElectricians");
        var $owner = $("#JobOwner");
        var $jobInstallation = $("#JobInstallationAddress");
        var $installer = $("#Installer");
        var $soldBy = $("#SoldBy");
        var items = $form.serializeArray();
        items.pushArray($installation.serializeArray());
        items.pushArray($electrician.serializeArray());
        items.pushArray($owner.serializeArray());
        items.pushArray($jobInstallation.serializeArray());
        items.pushArray($installer.serializeArray());
        items.pushArray($soldBy.serializeArray());
        var returnObj = {};
        var nestedObjectNames = [];



        $.each(items, function (i, item) {
            //Split nested objects and assign properties
            //You may want to make this recursive - currently only works one step deep, but that's all I need
            if (item.name == 'BasicDetails.strInstallationDate') {
                if (item.value != '' && item.value != undefined && item.value != null) {
                    var installationTick = ConvertDateToTick(item.value, GetDateFormat);
                    installationDateSerialize = moment(installationTick).format("YYYY-MM-DD");
                    item.value = installationDateSerialize;
                }
            }

            //if (item.name=='BasicDetails.GSTDocument') {
            //    // find GSTDocument
            //    var GSTDoc = document.getElementsByName("GSTDoc");
            //    if (GSTDoc.length > 0) {
            //        item.value = $("#GSTDoc").val();
            //    }
            //}

            if (item.name == 'BasicDetails.strSoldByDate') {
                if (item.value != '' && item.value != undefined && item.value != null) {
                    var soldbyDateTick = ConvertDateToTick(item.value, GetDateFormat);
                    soldbyDatSerialize = moment(soldbyDateTick).format("YYYY-MM-DD");
                    item.value = soldbyDatSerialize;
                }
            }

            if (item.name.indexOf('.') != -1) {
                var nameArray = item.name.split('.');
                if (nestedObjectNames.indexOf(nameArray[0]) < 0) {
                    nestedObjectNames.push(nameArray[0]);
                }
                var tempObj = returnObj[nestedObjectNames[nestedObjectNames.indexOf(nameArray[0])]] || {};
                if (!tempObj[nameArray[1]]) {
                    tempObj[nameArray[1]] = item.value;
                }
                returnObj[nestedObjectNames[nestedObjectNames.indexOf(nameArray[0])]] = tempObj;
            } else if (!returnObj[item.name]) {
                returnObj[item.name] = item.value;
            }
        });

        return returnObj;
    };

    $('#JobInstallationDetails_ExistingSystem').change(function () {
        if ($(this).is(":checked")) {
            $("#ExistingSystem").show();
        } else {
            $("#ExistingSystem").hide();
        }

    });

    $('#BasicDetails_IsGst').change(function () {

        $("#tblDocuments").find('tr').each(function () {
            $(this).remove();
        });

        if ($(this).is(":checked")) {
            $("#jobGST").show();
        } else {
            $("#jobGST").hide();
        }
    });


    $("#JobInstallationDetails_Town").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: urlProcessRequest,
                dataType: 'json',
                data: {
                    excludePostBoxFlag: true,
                    q: request.term
                },
                success: function (data) {
                    var data1 = JSON.parse(data);
                    if (data1.localities.locality instanceof Array)
                        response($.map(data1.localities.locality, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.location,
                                state: item.state,
                                postcode: item.postcode
                            }
                        }));
                    else
                        response($.map(data1.localities, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.location,
                                state: item.state,
                                postcode: item.postcode
                            }
                        }));
                }
            })
        },
        select: function (event, ui) {
            $('#JobInstallationDetails_State').val(ui.item.state);
            $('#JobInstallationDetails_PostCode').val(ui.item.postcode);
        }
    });

    $("#JobInstallationDetails_PostCode").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: urlProcessRequest,
                dataType: 'json',
                data: {
                    excludePostBoxFlag: true,
                    q: request.term
                },
                success: function (data) {
                    var data1 = JSON.parse(data);
                    if (data1.localities.locality instanceof Array)
                        response($.map(data1.localities.locality, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.postcode,
                                state: item.state,
                                location: item.location
                            }
                        }));
                    else
                        response($.map(data1.localities, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.postcode,
                                state: item.state,
                                location: item.location
                            }
                        }));
                }
            })
        },
        select: function (event, ui) {
            $('#JobInstallationDetails_State').val(ui.item.state);
            $('#JobInstallationDetails_Town').val(ui.item.location);
        }
    });


    $("#JobOwnerDetails_Town").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: urlProcessRequest,
                dataType: 'json',
                data: {
                    excludePostBoxFlag: true,
                    q: request.term
                },
                success: function (data) {
                    var data1 = JSON.parse(data);
                    if (data1.localities.locality instanceof Array)
                        response($.map(data1.localities.locality, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.location,
                                state: item.state,
                                postcode: item.postcode
                            }
                        }));
                    else
                        response($.map(data1.localities, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.location,
                                state: item.state,
                                postcode: item.postcode
                            }
                        }));
                }
            })
        },
        select: function (event, ui) {
            $('#JobOwnerDetails_State').val(ui.item.state);
            $('#JobOwnerDetails_PostCode').val(ui.item.postcode);
        }
    });

    $("#JobOwnerDetails_PostCode").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: urlProcessRequest,
                dataType: 'json',
                data: {
                    excludePostBoxFlag: true,
                    q: request.term
                },
                success: function (data) {
                    var data1 = JSON.parse(data);
                    if (data1.localities.locality instanceof Array)
                        response($.map(data1.localities.locality, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.postcode,
                                state: item.state,
                                location: item.location
                            }
                        }));
                    else
                        response($.map(data1.localities, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.postcode,
                                state: item.state,
                                location: item.location
                            }
                        }));
                }
            })
        },
        select: function (event, ui) {
            $('#JobOwnerDetails_State').val(ui.item.state);
            $('#JobOwnerDetails_Town').val(ui.item.location);
        }
    });


    $("#JobElectricians_Town").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: urlProcessRequest,
                dataType: 'json',
                data: {
                    excludePostBoxFlag: true,
                    q: request.term
                },
                success: function (data) {
                    var data1 = JSON.parse(data);
                    //$('#JobElectricians_State').val(data1.localities.locality[0].location);
                    //$('#JobElectricians_State').val(data1.localities.locality[0].state);
                    //$('#JobElectricians_PostCode').val(data1.localities.locality[0].postcode);
                    console.log(data1);
                    if (data1.localities.locality instanceof Array)
                        response($.map(data1.localities.locality, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.location,
                                state: item.state,
                                postcode: item.postcode
                            }
                        }));
                    else
                        response($.map(data1.localities, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.location,
                                state: item.state,
                                postcode: item.postcode
                            }
                        }));
                }
            })
        },
        select: function (event, ui) {
            $('#JobElectricians_State').val(ui.item.state);
            $('#JobElectricians_PostCode').val(ui.item.postcode);
        }
    });

    $("#JobElectricians_PostCode").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: urlProcessRequest,
                dataType: 'json',
                data: {
                    excludePostBoxFlag: true,
                    q: request.term
                },
                success: function (data) {
                    var data1 = JSON.parse(data);
                    if (data1.localities.locality instanceof Array)
                        response($.map(data1.localities.locality, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.postcode,
                                state: item.state,
                                location: item.location
                            }
                        }));
                    else
                        response($.map(data1.localities, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.postcode,
                                state: item.state,
                                location: item.location
                            }
                        }));
                }
            })
        },
        select: function (event, ui) {
            $('#JobElectricians_State').val(ui.item.state);
            $('#JobElectricians_Town').val(ui.item.location);
        }
    });

    $("#JobInstallerDetails_Town").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: urlProcessRequest,
                dataType: 'json',
                data: {
                    excludePostBoxFlag: true,
                    q: request.term
                },
                success: function (data) {
                    var data1 = JSON.parse(data);
                    //$('#JobElectricians_State').val(data1.localities.locality[0].location);
                    //$('#JobElectricians_State').val(data1.localities.locality[0].state);
                    //$('#JobElectricians_PostCode').val(data1.localities.locality[0].postcode);

                    if (data1.localities.locality instanceof Array)
                        response($.map(data1.localities.locality, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.location,
                                state: item.state,
                                postcode: item.postcode
                            }
                        }));
                    else
                        response($.map(data1.localities, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.location,
                                state: item.state,
                                postcode: item.postcode
                            }
                        }));
                }
            })
        },
        select: function (event, ui) {
            $('#JobInstallerDetails_State').val(ui.item.state);
            $('#JobInstallerDetails_PostCode').val(ui.item.postcode);
        }
    });

    $("#JobInstallerDetails_PostCode").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: urlProcessRequest,
                dataType: 'json',
                data: {
                    excludePostBoxFlag: true,
                    q: request.term
                },
                success: function (data) {
                    var data1 = JSON.parse(data);
                    if (data1.localities.locality instanceof Array)
                        response($.map(data1.localities.locality, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.postcode,
                                state: item.state,
                                location: item.location
                            }
                        }));
                    else
                        response($.map(data1.localities, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.postcode,
                                state: item.state,
                                location: item.location
                            }
                        }));
                }
            })
        },
        select: function (event, ui) {
            $('#JobInstallerDetails_State').val(ui.item.state);
            $('#JobInstallerDetails_Town').val(ui.item.location);
        }
    });
    $('#btnAddressDetail').click(function () {

        $('#popupAddress').modal({ backdrop: 'static', keyboard: false });

        setTimeout(function () {
            $('#JobInstallationDetails_AddressID').focus();
        }, 1000);

        $(".popupAddress").find('input[type=text]').each(function () {

            $(this).attr('class', 'form-control valid');
        });
        $(".popupAddress").find('.field-validation-error').attr('class', 'field-validation-valid');
        $.each(InstallationJson, function (key, value) {

            $('#JobInstallationDetails_AddressID').val(value.PostalAddressType);

            $('#JobInstallationDetails_UnitNumber').val(value.UnitNumber);
            $('#JobInstallationDetails_UnitTypeID').val(value.UnitType);
            $('#JobInstallationDetails_StreetNumber').val(value.StreetNumber);
            $('#JobInstallationDetails_StreetName').val(value.StreetName);
            $('#JobInstallationDetails_StreetTypeID').val(value.StreetType);
            $('#JobInstallationDetails_Town').val(value.Town);
            $('#JobInstallationDetails_State').val(value.State);
            $('#JobInstallationDetails_PostCode').val(value.PostCode);

            $("#JobInstallationDetails_PostalDeliveryNumber").val(value.PostalDeliveryNumber);
            $('#JobInstallationDetails_PostalAddressID').val(value.PostalAddressID);


        });



        if ($('#JobInstallationDetails_UnitTypeID option:selected').val() == "") {
            $('#lblInstallationUnitNumber').removeClass("required");
            $('#lblInstallationUnitTypeID').removeClass("required");
            $('#lblInstallationStreetNumber').addClass("required");
        }
        else {
            $('#lblInstallationUnitNumber').addClass("required");
            $('#lblInstallationUnitTypeID').addClass("required");
            //$('#lblInstallationStreetNumber').removeClass("required");
        }

        if ($('#JobInstallationDetails_AddressID').val() == 1) {
            $('.InstallationDPA').show();
            $('.InstallationPDA').hide();
        }
        else {
            $('.InstallationPDA').show();
            $('.InstallationDPA').hide();
        }
    });


    $('#btnClearInsatallerDetail').click(function () {
        clearPopupInstaller();
        InstallerJson = [];
        var Installername;
        var FirstName = $("#JobInstallerDetails_FirstName").val();
        var Surname = $("#JobInstallerDetails_Surname").val();
        Installername = FirstName + ' ' + Surname;
        $('#spantxtClientNames').hide();
        $("#txtInstallerName").val(Installername);
        if ($("#JobInstallerDetails_AddressID").val() == 1) {
            InstallerJson.push({ Phone: $('#JobInstallerDetails_Phone').val(), Mobile: $('#JobInstallerDetails_Mobile').val(), FirstName: $('#JobInstallerDetails_FirstName').val(), Surname: $('#JobInstallerDetails_Surname').val(), Email: $('#JobInstallerDetails_Email').val(), PostalAddressType: $("#JobInstallerDetails_AddressID").val(), UnitType: $("#JobInstallerDetails_UnitTypeID").val(), UnitNumber: $("#JobInstallerDetails_UnitNumber").val(), StreetNumber: $("#JobInstallerDetails_StreetNumber").val(), StreetName: $("#JobInstallerDetails_StreetName").val(), StreetType: $("#JobInstallerDetails_StreetTypeID").val(), Town: $("#JobInstallerDetails_Town").val(), State: $("#JobInstallerDetails_State").val(), PostCode: $("#JobInstallerDetails_PostCode").val() });
        } else {
            InstallerJson.push({ Phone: $('#JobInstallerDetails_Phone').val(), Mobile: $('#JobInstallerDetails_Mobile').val(), FirstName: $('#JobInstallerDetails_FirstName').val(), Surname: $('#JobInstallerDetails_Surname').val(), Email: $('#JobInstallerDetails_Email').val(), PostalAddressType: $("#JobInstallerDetails_AddressID").val(), PostalAddressID: $("#JobInstallerDetails_PostalAddressID").val(), PostalDeliveryNumber: $("#JobInstallerDetails_PostalDeliveryNumber").val(), Town: $("#JobInstallerDetails_Town").val(), State: $("#JobInstallerDetails_State").val(), PostCode: $("#JobInstallerDetails_PostCode").val() });

        }
        InstallerEmailFromJson = $('#JobInstallerDetails_Email').val();
        InstallerFirstNameFromJson = $("#JobInstallerDetails_FirstName").val();
        InstallerLastNameFromJson = $("#JobInstallerDetails_Surname").val();
    });

    $('#btnClearBasicAddressDetail').click(function () {
        clearPopupBasicAddress();
        ElectrianAddressJson = [];
        var address;
        var UnitTypeId = $("#JobElectricians_UnitTypeID").find("option:selected").text();
        var UnitNumber = $("#JobElectricians_UnitNumber").val();
        var StreetNumber = $("#JobElectricians_StreetNumber").val();
        var StreetName = $("#JobElectricians_StreetName").val();
        var StreetTypeId = $("#JobElectricians_StreetTypeID").find("option:selected").text();
        var PostalAddressID = $("#JobElectricians_PostalAddressID").find("option:selected").text();
        var PostalDeliveryNumber = $("#JobElectricians_PostalDeliveryNumber").val();
        var Town = $("#JobElectricians_Town").val();
        var State = $("#JobElectricians_State").val();
        var PostCode = $("#JobElectricians_PostCode").val();
        //bansi4
        var companyName = $('#JobElectricians_CompanyName').val();
        var firstName = $('#JobElectricians_FirstName').val();
        var lastName = $('#JobElectricians_LastName').val();
        var email = $('#JobElectricians_Email').val();
        var phone = $('#JobElectricians_Phone').val();
        var mobile = $('#JobElectricians_Mobile').val();
        var licenseNumber = $('#JobElectricians_LicenseNumber').val();

        if ($("#JobElectricians_AddressID").val() == 1) {
            if (UnitNumber != "") {
                address = UnitTypeId + ' ' + UnitNumber + "/" + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                address = address.replace("Select", "");
            } else {
                address = UnitTypeId + ' ' + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                address = address.replace("Select", "");
            }
            ElectrianAddressJson.push({ PostalAddressType: $("#JobElectricians_AddressID").val(), UnitType: $("#JobElectricians_UnitTypeID").val(), UnitNumber: $("#JobElectricians_UnitNumber").val(), StreetNumber: $("#JobElectricians_StreetNumber").val(), StreetName: $("#JobElectricians_StreetName").val(), StreetType: $("#JobElectricians_StreetTypeID").val(), Town: $("#JobElectricians_Town").val(), State: $("#JobElectricians_State").val(), PostCode: $("#JobElectricians_PostCode").val(), CompanyName: companyName, FirstName: firstName, LastName: lastName, Email: email, Phone: phone, Mobile: mobile, LicenseNumber: licenseNumber });

        } else {
            address = PostalAddressID + ' ' + PostalDeliveryNumber + ', ' + Town + ' ' + State + ' ' + PostCode;
            ElectrianAddressJson.push({ PostalAddressType: $("#JobElectricians_AddressID").val(), PostalAddressID: $("#JobElectricians_PostalAddressID").val(), PostalDeliveryNumber: $("#JobElectricians_PostalDeliveryNumber").val(), Town: $("#JobElectricians_Town").val(), State: $("#JobElectricians_State").val(), PostCode: $("#JobElectricians_PostCode").val(), CompanyName: companyName, FirstName: firstName, LastName: lastName, Email: email, Phone: phone, Mobile: mobile, LicenseNumber: licenseNumber })
        }

        $("#txtBasicAddress").val($('#JobElectricians_FirstName').val() + ' ' + $('#JobElectricians_LastName').val());
        $('#spantxtBasicAddress').hide();
    });

    $('#btnBasicAddressDetail').click(function () {

        $('#popupBasicAddress').modal({ backdrop: 'static', keyboard: false });
        setTimeout(function () {
            $('#JobElectricians_InstallerID').focus();
        }, 1000);

        $(".popupBasicAddress").find('input[type=text]').each(function () {

            $(this).attr('class', 'form-control valid');
        });
        $(".popupBasicAddress").find('.field-validation-error').attr('class', 'field-validation-valid');
        $.each(ElectrianAddressJson, function (key, value) {
            $("#JobElectricians_CompanyName").val(value.CompanyName);
            $("#JobElectricians_FirstName").val(value.FirstName);
            $("#JobElectricians_LastName").val(value.LastName);
            $("#JobElectricians_Email").val(value.Email);
            $("#JobElectricians_Phone").val(value.Phone);
            $("#JobElectricians_Mobile").val(value.Mobile);
            $("#JobElectricians_LicenseNumber").val(value.LicenseNumber);
            $('#JobElectricians_AddressID').val(value.PostalAddressType);
            $('#JobElectricians_UnitNumber').val(value.UnitNumber);
            $('#JobElectricians_UnitTypeID').val(value.UnitType);
            $('#JobElectricians_StreetNumber').val(value.StreetNumber);
            $('#JobElectricians_StreetName').val(value.StreetName);
            $('#JobElectricians_StreetTypeID').val(value.StreetType);
            $('#JobElectricians_Town').val(value.Town);
            $('#JobElectricians_State').val(value.State);
            $('#JobElectricians_PostCode').val(value.PostCode);
            $("#JobElectricians_PostalDeliveryNumber").val(value.PostalDeliveryNumber);
            $('#JobElectricians_PostalAddressID').val(value.PostalAddressID);
        });

        if ($('#JobElectricians_UnitTypeID option:selected').val() == "") {
            $('#lblElectriciansUnitNumber').removeClass("required");
            $('#lblElectriciansUnitTypeID').removeClass("required");
            $('#lblElectriciansStreetNumber').addClass("required");
        }
        else {
            $('#lblElectriciansUnitNumber').addClass("required");
            $('#lblElectriciansUnitTypeID').addClass("required");
            $('#lblElectriciansStreetNumber').removeClass("required");
        }

        if ($('#JobElectricians_AddressID').val() == 1) {
            $('.ElectriciansDPA').show();
            $('.ElectriciansPDA').hide();
        }
        else {
            $('.ElectriciansPDA').show();
            $('.ElectriciansDPA').hide();
        }
    });

    var isMapsApiLoaded = false;
    window.mapsCallback = function () {
        isMapsApiLoaded = true;
        loadMap();
        // initialize map, etc.
    };

    $('#btnMap').click(function () {
        //if (window.google != undefined && (typeof google === 'object' && typeof google.maps === 'object')) {
        if (isMapsApiLoaded) {
            loadMap();
        }
        else {
            var scriptMap = document.createElement("script");
            scriptMap.type = "text/javascript";
            scriptMap.async = false;

            scriptMap.src = JobMapKeyUrl.toString().replace(/&amp;/g, '&');

            document.body.appendChild(scriptMap);

            //loadMap();
        }

        $('#popupMap').modal({ backdrop: 'static', keyboard: false });

        setTimeout(function () {
            $('#txtSource').focus();
        }, 1000);
        $('#txtSource').val('');

        $("#dvDistance").html('');
        $("#errorMsgRegionMap").hide();
        var address = $('#txtAddress').val();

        $('#txtDestination').val(address);
        var a = setTimeout(function () {
            if (ProjectSession_UserTypeId == 1) {
                geocodeAddress(address);
            }
        }, 1000);
    });

    $('#btnOwnerDetails').click(function () {
        $('#popupOwner').modal({ backdrop: 'static', keyboard: false });
        setTimeout(function () {
            $('#JobOwnerDetails_OwnerType').focus();
            $("#JobOwnerDetails_OwnerType").change();

        }, 1000);

        $(".popupOwner").find('input[type=text]').each(function () {

            $(this).attr('class', 'form-control valid');
        });
        $(".popupOwner").find('.field-validation-error').attr('class', 'field-validation-valid');

        //todo

        $.each(OwnerAddressJson, function (key, value) {
            $('#JobOwnerDetails_OwnerType').val(value.OwnerType);
            $("#JobOwnerDetails_CompanyName").val(value.CompanyName);
            $("#JobOwnerDetails_FirstName").val(value.FirstName);
            $('#JobOwnerDetails_LastName').val(value.LastName);
            $('#JobOwnerDetails_Email').val(value.Email);
            $('#JobOwnerDetails_Phone').val(value.Phone);
            $('#JobOwnerDetails_Mobile').val(value.Mobile);

            $('#JobOwnerDetails_AddressID').val(value.PostalAddressType);
            $('#JobOwnerDetails_UnitNumber').val(value.UnitNumber);
            $('#JobOwnerDetails_UnitTypeID').val(value.UnitType);
            $('#JobOwnerDetails_StreetNumber').val(value.StreetNumber);
            $('#JobOwnerDetails_StreetName').val(value.StreetName);
            $('#JobOwnerDetails_StreetTypeID').val(value.StreetType);
            $('#JobOwnerDetails_Town').val(value.Town);
            $('#JobOwnerDetails_State').val(value.State);
            $('#JobOwnerDetails_PostCode').val(value.PostCode);

            $("#JobOwnerDetails_PostalDeliveryNumber").val(value.PostalDeliveryNumber);
            $('#JobOwnerDetails_PostalAddressID').val(value.PostalAddressID);

        });

        if ($('#JobOwnerDetails_UnitTypeID option:selected').val() == "") {
            $('#lblOwnerUnitNumber').removeClass("required");
            $('#lblOwnerUnitTypeID').removeClass("required");
            $('#lblOwnerStreetNumber').addClass("required");
        }
        else {
            $('#lblOwnerUnitNumber').addClass("required");
            $('#lblOwnerUnitTypeID').addClass("required");
            $('#lblOwnerStreetNumber').removeClass("required");
        }
        if ($('#JobOwnerDetails_AddressID').val() == 1) {
            $('.OwnerDPA').show();
            $('.OwnerPDA').hide();
        }
        else {
            $('.OwnerPDA').show();
            $('.OwnerDPA').hide();
        }


    });

    $('#btnInstallerDetails').click(function () {    
        $('#popupInstaller').modal({ backdrop: 'static', keyboard: false });
        setTimeout(function () {
            $('#JobInstallerDetails_FirstName').focus();
        }, 1000);
        $(".popupInstaller").find('input[type=text]').each(function () {

            $(this).attr('class', 'form-control valid');
        });
        $(".popupInstaller").find('.field-validation-error').attr('class', 'field-validation-valid');
        
        $('#JobInstallerDetails_ElectricianID').val($('#JobInstallerDetails_SWHInstallerDesignerId').val());
        $('#JobInstallerDetails_LicenseNumber').val($('#JobInstallerDetails_LicenseNumber').val());
        $("#JobInstallerDetails_LicenseNumber").attr("readonly", "readonly");
        $('#JobInstallerDetails_LicenseNumber').attr('onblur', '')

        if ($('#JobInstallerDetails_SESignature').val() != null && $('#JobInstallerDetails_SESignature').val() != '') {
            var SignName = $('#JobInstallerDetails_SESignature').val();
            var guid = ProjectSession_LoggedInUserId;
            var proofDocumentURL = UploadedDocumentPath;
            var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
            SRCSignSWHInstall = imagePath + "/" + SignName;
            $('#imgSignSWHInstall').attr('class', SignName);
            $('#imgSWHSignatureInstaller').show();
        }
        else
            $('#imgSWHSignatureInstaller').hide();

        $.each(InstallerJson, function (key, value) {

            $("#JobInstallerDetails_FirstName").val(value.FirstName);
            $('#JobInstallerDetails_Surname').val(value.Surname);
            $('#JobInstallerDetails_Email').val(value.Email);
            $('#JobInstallerDetails_Phone').val(value.Phone);
            $('#JobInstallerDetails_Mobile').val(value.Mobile);

            $('#JobInstallerDetails_AddressID').val(value.PostalAddressType);
            $('#JobInstallerDetails_UnitNumber').val(value.UnitNumber);
            $('#JobInstallerDetails_UnitTypeID').val(value.UnitType);
            $('#JobInstallerDetails_StreetNumber').val(value.StreetNumber);
            $('#JobInstallerDetails_StreetName').val(value.StreetName);
            $('#JobInstallerDetails_StreetTypeID').val(value.StreetType);
            $('#JobInstallerDetails_Town').val(value.Town);
            $('#JobInstallerDetails_State').val(value.State);
            $('#JobInstallerDetails_PostCode').val(value.PostCode);

            $("#JobInstallerDetails_PostalDeliveryNumber").val(value.PostalDeliveryNumber);
            $('#JobInstallerDetails_PostalAddressID').val(value.PostalAddressID);
            $("#JobInstallerDetails_SolarCompanyId").val(value.SolarCompanyId);
            $('#JobInstallerDetails_SESignature').val(value.SESignature);
        });

        if ($('#JobInstallerDetails_UnitTypeID option:selected').val() == "") {
            $('#lblInstallerUnitNumber').removeClass("required");
            $('#lblInstallerUnitTypeID').removeClass("required");
            $('#lblInstallerStreetNumber').addClass("required");
        }
        else {
            $('#lblInstallerUnitNumber').addClass("required");
            $('#lblInstallerUnitTypeID').addClass("required");
            $('#lblInstallerStreetNumber').removeClass("required");
        }
        if ($('#JobInstallerDetails_AddressID').val() == 1) {
            $('.InstallerDPA').show();
            $('.InstallerPDA').hide();
        }
        else {
            $('.InstallerPDA').show();
            $('.InstallerDPA').hide();
        }

    });

    $('#JobInstallationDetails_NoOfPanels').blur(function () {
        if (parseInt($(this).val()) > 10000) {
            $('#spanJobInstallationDetailsNoOfPanelValue').show();
            $(this).val('');
        }
        else {
            $('#spanJobInstallationDetailsNoOfPanelValue').hide();
        }
        if (parseInt($(this).val()) < 1) {

            $(this).val('');
        }
    });

    $('#JobSystemDetails_SystemSize').blur(function () {
        if ($(this).val() == "" || parseFloat($(this).val()) <= 0) {
            $('#spanJobSystemDetails_SystemSize').show();
        }
        else {
            $('#spanJobSystemDetails_SystemSize').hide();
        }
    });
    //$('#JobPanelDetails_NoOfPanel').blur(function() {
    //    if(parseInt($(this).val()) >10000  ) {
    //        $('#spanNoOfPanelValue').show();
    //        $(this).val('');
    //    }
    //    else
    //    {
    //        $('#spanNoOfPanelValue').hide();
    //    }
    //});

    $('#JobSystemDetails_SystemSize').blur(function () {
        if ($(this).val() == "" || parseFloat($(this).val()) <= 0) {
            $('#spanJobSystemDetails_SystemSize').show();
        }
        else {
            $('#spanJobSystemDetails_SystemSize').hide();
        }
    });

    $('#JobPanelDetails_NoOfPanel').blur(function () {
        if (parseInt($(this).val()) > 10000) {
            $('#spanNoOfPanelValue').show();
            $(this).val('');
        }
        else {
            $('#spanNoOfPanelValue').hide();
        }

        if (parseInt($(this).val()) < 1) {

            $(this).val('');
        }
    });

    $('#JobSystemDetails_NoOfPanel').blur(function () {

        if (parseInt($(this).val()) > 10000) {
            $('#spanNoOfPanelSystemValue').show();
            $(this).val('');
        }
        else {
            $('#spanNoOfPanelSystemValue').hide();
        }
    });


    //$('#JobSTCDetails_ExplanatoryNotes').blur(function() {
    //    if($(this).val() != "" && $(this).val() != null && $(this).val() != undefined ) {
    //        $("#spanExplanatoryNotes").hide();
    //    }

    //});
    $('#JobSTCDetails_FailedAccreditationCode').blur(function () {
        if ($(this).val() != "" && $(this).val() != null && $(this).val() != undefined) {
            $("#spanFailedAccreditationCode").hide();
        }

    });



    //$('#JobSTCDetails_AdditionalCapacityNotes').blur(function() {
    //    if($(this).val() != "" && $(this).val() != null && $(this).val() != undefined ) {
    //        $("#spanAdditionalCapacityNotes").hide();
    //        $("#spanAdditionalCapacityNotesMinimum").hide();
    //    }

    //});

    //$("#JobSTCDetails_InstallingCompleteUnit").change(function(){
    //    $("#spanAdditionalCapacityNotes").hide();
    //    $('#JobSTCDetails_AdditionalCapacityNotes').val('');
    //});

    $('#JobSTCDetails_StatutoryDeclarations').change(function () {
        if ($(this).val() != "Select" && $(this).val() != null && $(this).val() != undefined && $(this).val() != "") {
            $("#spanStatutoryDeclarations").hide();
        }
    });

    $("#JobSTCDetails_VolumetricCapacity").change(function () {
        $("#spanStatutoryDeclarations").hide();
        if ($(this).val() == "Yes") {
            $("#divStatutoryDeclarations").show();
        } else {
            $("#divStatutoryDeclarations").val($("#divStatutoryDeclarations option:first").val());
            $("#divStatutoryDeclarations").hide();
        }
    });

    $(".OwnerAddress").change(function () {
        $(".OwnerDPA").find('input[type=text]').each(function () {
            $(this).val('');
            $(this).attr('class', 'form-control valid');
        });
        $(".OwnerDPA").find('Select').each(function () {

            $(this).find('option:first').attr('selected', 'selected');
        });
        $(".OwnerDPA").find('.field-validation-error').attr('class', 'field-validation-valid');
        //PDA
        $(".OwnerPDA").find('input[type=text]').each(function () {
            $(this).val('');
            $(this).attr('class', 'form-control valid');
        });
        $(".OwnerPDA").find('Select').each(function () {

            $(this).find('option:first').attr('selected', 'selected');
        });
        $(".OwnerPDA").find('.field-validation-error').attr('class', 'field-validation-valid');

        if ($(this).val() == 1) {

            $('.OwnerDPA').show();
            $('.OwnerPDA').hide();
        }
        else {
            $('.OwnerPDA').show();
            $('.OwnerDPA').hide();
        }
    });

    $(".InstallerAddress").change(function () {

        $(".InstallerDPA").find('Select').each(function () {

            $(this).find('option:first').attr('selected', 'selected');
        });
        $(".InstallerDPA").find('input[type=text]').each(function () {
            $(this).val('');
            $(this).attr('class', 'form-control valid');
        });

        $(".InstallerDPA").find('.field-validation-error').attr('class', 'field-validation-valid');
        //PDA

        $(".InstallerPDA").find('Select').each(function () {

            $(this).find('option:first').attr('selected', 'selected');
        });
        $(".InstallerPDA").find('input[type=text]').each(function () {
            $(this).val('');
            $(this).attr('class', 'form-control valid');
        });

        $(".InstallerPDA").find('.field-validation-error').attr('class', 'field-validation-valid');

        if ($(this).val() == 1) {
            $('.InstallerDPA').show();
            $('.InstallerPDA').hide();
        }
        else {
            $('.InstallerPDA').show();
            $('.InstallerDPA').hide();
        }
    });

    $(".InstallationAddress").change(function () {

        $(".InstallationDPA").find('Select').each(function () {

            $(this).find('option:first').attr('selected', 'selected');
        });
        $(".InstallationDPA").find('input[type=text]').each(function () {
            $(this).val('');
            $(this).attr('class', 'form-control valid');
        });

        $(".InstallationDPA").find('.field-validation-error').attr('class', 'field-validation-valid');

        //PDA

        $(".InstallationPDA").find('Select').each(function () {

            $(this).find('option:first').attr('selected', 'selected');
        });
        $(".InstallationPDA").find('input[type=text]').each(function () {
            $(this).val('');
            $(this).attr('class', 'form-control valid');
        });

        $(".InstallationPDA").find('.field-validation-error').attr('class', 'field-validation-valid');

        if ($(this).val() == 1) {
            $('.InstallationDPA').show();
            $('.InstallationPDA').hide();
        }
        else {
            $('.InstallationPDA').show();
            $('.InstallationDPA').hide();
        }
    });

    $(".ElectriciansAddress").change(function () {
        $(".ElectriciansDPA").find('Select').each(function () {
            $(this).find('option:first').attr('selected', 'selected');
        });

        $(".ElectriciansDPA").find('input[type=text]').each(function () {
            $(this).val('');
            $(this).attr('class', 'form-control valid');
        });

        $(".ElectriciansDPA").find('.field-validation-error').attr('class', 'field-validation-valid');

        //PDA

        $(".ElectriciansPDA").find('Select').each(function () {

            $(this).find('option:first').attr('selected', 'selected');
        });
        $(".ElectriciansPDA").find('input[type=text]').each(function () {
            $(this).val('');
            $(this).attr('class', 'form-control valid');
        });

        $(".ElectriciansPDA").find('.field-validation-error').attr('class', 'field-validation-valid');


        if ($(this).val() == 1) {
            $('.ElectriciansDPA').show();
            $('.ElectriciansPDA').hide();
        }
        else {
            $('.ElectriciansPDA').show();
            $('.ElectriciansDPA').hide();
        }
    });



    //Datatable

    //Generate New GUID
    function Fuid() {

        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
              .toString(16)
              .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
          s4() + '-' + s4() + s4() + s4();
    }

    $('#btnAddPanel').click(function () {        
        var panelBrand = $("#JobPanelDetails_Brand").val();
        var panelModel = $("#JobPanelDetails_Model").val();
        var noOfPanel = $("#JobPanelDetails_NoOfPanel").val();

        //var isDuplicate = false;


        if ((panelBrand != null && panelBrand != "") && (panelModel != null && panelModel != "") && (noOfPanel != null && noOfPanel != "" && noOfPanel != 0)) {
            var trId = Fuid();
            var trID = trId;
            var count = 1;
            var content = "<tr id='" + trID + "'>"
            content += '<td class="' + panelBrand + '"  width="40%">' + panelBrand + ' </td>';
            content += '<td class="' + panelModel + '"  width="35%">' + panelModel + ' </td>';
            content += '<td class="' + noOfPanel + '"  width="10%" >' + noOfPanel + ' </td>';
            content += '<td  class="' + 'action' + " " +trID + '" id="tdAction"  width="15%"><a class="edit sprite-img" title="Edit" id="signEdit" style="cursor: pointer" href="javascript:void(0)" onclick="editPanel(\'' + trID + '\')"></a>&nbsp;&nbsp;<a class="delete sprite-img" title="Delete" id="signDelete" style="cursor: pointer" onclick="DeletePanelFromGrid(\'' + trID + '\')"></a></td>';
            content += "</tr>"
            $('#tblPanel').append(content);
            $('#gridPanel').show();
            var panel = 0;
            if ($("#JobSystemDetails_NoOfPanel").val() != '') {
                panel = $("#JobSystemDetails_NoOfPanel").val();
            }
            $("#JobSystemDetails_NoOfPanel").val(parseInt(panel) + parseInt(noOfPanel));
            PanelXml.push({ ID: trID, Brand: panelBrand, Model: panelModel, Count: noOfPanel });
            //PanelXml.push(panel);
        }
        else {
            if (panelBrand == null || panelBrand.trim() == "") {
                $("#spanJobPanelDetails_Brand").show().fadeOut(3000);
            }

            if (panelModel == null || panelModel.trim() == "") {
                $("#spanJobPanelDetails_Model").show().text('Panel Model  is required..').fadeOut(3000);
            }

            if (noOfPanel == null || noOfPanel.trim() == "" || noOfPanel == "0") {
                $("#spanNoOfPanelValue").show().fadeOut(5000);
            }

        }
    });

    $('#btnAddInverter').click(function () {

        var inverterBrand = $("#JobInverterDetails_Brand").val();
        var inverterModel = $("#JobInverterDetails_Model").val();
        var series = $("#JobInverterDetails_Series").val();

        var isDuplicate = false;

        if ((inverterBrand != null && inverterBrand != "") && (inverterModel != null && inverterModel != "") && (series != null && series != "")) {
            //for (var i = 0; i < InverterXml.length; i++) {
            //    if(InverterXml[i].Series == series && InverterXml[i].Brand == inverterBrand && InverterXml[i].Model == inverterModel)
            //    {
            //        isDuplicate = true;
            //        break;
            //    }
            //}


            //if (isDuplicate) {
            //    $("#spanJobInverterDetails_Series").show().text('Same Inverter is exist.').fadeOut(5000);
            //    return;
            //}

            var trId = Fuid();
            var count = 1;
            var content = "<tr id='+ trId +'>"
            content += '<td class="' + inverterBrand + '"  width="35%">' + inverterBrand + ' </td>';
            content += '<td class="' + series + '"  width="25%">' + series + ' </td>';
            content += '<td class="' + inverterModel + '" width="25%">' + inverterModel + ' </td>';
            content += '<td  class="' + 'action' + " " + trId + '" id="tdAction"  width="15%" ><a class="edit sprite-img" title="Edit" id="signEdit" style="cursor: pointer" href="javascript:void(0)" onclick="editInverter(\'' + trId + '\')"></a>&nbsp;&nbsp;<a class="delete sprite-img" title="Delete" id="signDelete" style="cursor: pointer" onclick="DeleteInverterFromGrid(\'' + trId + '\')"></a></td>';
            content += "</tr>"
            $('#tblInverter').append(content);
            $('#gridInverter').show();



            InverterXml.push({ ID: trId, Brand: inverterBrand, Model: inverterModel, Series: series });
        }
        else {
            if (inverterBrand == null || inverterBrand.trim() == "")
                $("#spanJobInverterDetails_Brand").show().fadeOut(3000);

            if (inverterModel == null || inverterModel.trim() == "")
                $("#spanJobInverterDetails_Model").show().fadeOut(3000);

            if (series == null || series.trim() == "")
                $("#spanJobInverterDetails_Series").show().text("Inverter Series  is required.").fadeOut(3000);
        }
    });

    $('#btnAddBatteryManfacturer').click(function () {

        var batteryManufacturer = $("#JobBatteryManufacturer_Manufacturer").val();
        var batteryModel = $("#JobBatteryManufacturer_ModelNumber").val();

        if ((batteryManufacturer != null && batteryManufacturer != "") && (batteryModel != null && batteryModel != "")) {
          
            var trId = Fuid();
            var count = 1;
            var content = "<tr id='+ trId +'>"
            content += '<td class="' + batteryManufacturer + '"  width="35%">' + batteryManufacturer + ' </td>';
            content += '<td class="' + batteryModel + '" width="25%">' + batteryModel + ' </td>';
            content += '<td  class="' + 'action' + " " + trId + '" id="tdAction"  width="15%" ><a class="edit sprite-img" title="Edit" id="signEdit" style="cursor: pointer" href="javascript:void(0)" onclick="editBatteryManufacturer(\'' + trId + '\')"></a>&nbsp;&nbsp;<a class="delete sprite-img" title="Delete" id="signDelete" style="cursor: pointer" onclick="DeleteBatteryManufacturerFromGrid(\'' + trId + '\')"></a></td>';
            content += "</tr>"

            $('#tblBatteryManufacturer').append(content);
            $('#gridBattery').show();

            batteryXml.push({ JobBatteryManufacturerId: trId, Manufacturer: batteryManufacturer, ModelNumber: batteryModel });

        }
        else {
            if (batteryManufacturer == null || batteryManufacturer.trim() == "")
                $("#spanJobBatteryManufacturer").show();

            if (batteryModel == null || batteryModel.trim() == "")
                $("#spanJobBatteryModelNumber").show();
        }
    });

    $("#deletejob").click(function (e) {
        DeleteJobs();
    })
    $('#JobInstallationDetails_IsSameAsOwnerAddress').change(function () {
        if ($(this).is(":checked")) {

            $.each(OwnerAddressJson, function (key, value) {

                $('#JobInstallationDetails_UnitNumber').val(value.UnitNumber);
                $('#JobInstallationDetails_UnitTypeID').val(value.UnitType);
                $('#JobInstallationDetails_StreetNumber').val(value.StreetNumber);
                $('#JobInstallationDetails_StreetName').val(value.StreetName);
                $('#JobInstallationDetails_StreetTypeID').val(value.StreetType);
                $('#JobInstallationDetails_Town').val(value.Town);
                $('#JobInstallationDetails_State').val(value.State);
                $('#JobInstallationDetails_PostCode').val(value.PostCode);

                $("#JobInstallationDetails_PostalDeliveryNumber").val(value.PostalDeliveryNumber);
                $('#JobInstallationDetails_PostalAddressID').val(value.PostalAddressID);

            });

            if ($(".OwnerAddress").val() == 1) {
                $(".InstallationAddress").val(1);
                $('.InstallationDPA').show();
                $('.InstallationPDA').hide();
            }
            else {
                $(".InstallationAddress").val(2);
                $('.InstallationPDA').show();
                $('.InstallationDPA').hide();
            }
            $(".popupAddress").find('input[type=text]').each(function () {
                $(this).attr('class', 'form-control valid');
            });
            $(".popupAddress").find('.field-validation-error').attr('class', 'field-validation-valid');
            $('#spantxtAddress').hide();
            $('#popupAddress').modal('toggle');
        } else {
            $(".popupAddress").find('input[type=text]').each(function () {
                $(this).val('');
                $(this).attr('class', 'form-control valid');
            });
            $(".popupAddress").find('.field-validation-error').attr('class', 'field-validation-valid');
            $("#JobInstallationDetails_UnitTypeID").val($("#JobInstallationDetails_UnitTypeID option:first").val());
            $("#JobInstallationDetails_StreetTypeID").val($("#JobInstallationDetails_StreetTypeID option:first").val());
            $("#JobInstallationDetails_PostalAddressID").val($("#JobInstallationDetails_PostalAddressID option:first").val());

        }
    });

    $('#JobElectricians_InstallerID').change(function () {
        $('#JobElectricians_ElectricianID').val("");
        var id = $('#JobElectricians_InstallerID').val();
        $.ajax(
               {
                   url: urlGetElectricianDetailbyInstaller,
                   data: { Id: id },
                   contentType: 'application/json',
                   method: 'get',
                   success: function (data) {

                       if (data != false) {
                           var obj = JSON.parse(data);
                           $('#JobElectricians_CompanyName').val(obj[0].CompanyName);
                           $('#JobElectricians_FirstName').val(obj[0].FirstName);
                           $('#JobElectricians_LastName').val(obj[0].LastName);
                           $('#JobElectricians_Email').val(obj[0].Email);
                           $('#JobElectricians_Phone').val(obj[0].Phone);
                           $('#JobElectricians_Mobile').val(obj[0].Mobile);
                           $('#JobElectricians_LicenseNumber').val(obj[0].ElectricalContractorsLicenseNumber);
                           var ispostal = 'False';
                           if (obj[0].IsPostalAddress == 'False') {
                               ispostal = '1'
                               $('.ElectriciansPDA').hide();
                               $('.ElectriciansDPA').show();
                           } else {
                               ispostal = '2'
                               $('.ElectriciansDPA').hide();
                               $('.ElectriciansPDA').show();
                           }
                           $('#JobElectricians_AddressID').val(ispostal);
                           $('#JobElectricians_PostalAddressID').val(obj[0].PostalAddressID);
                           $('#JobElectricians_PostalDeliveryNumber').val(obj[0].PostalDeliveryNumber);
                           $('#JobElectricians_UnitTypeID').val(obj[0].UnitTypeID);
                           $('#JobElectricians_UnitNumber').val(obj[0].UnitNumber);
                           $('#JobElectricians_StreetNumber').val(obj[0].StreetNumber);
                           $('#JobElectricians_StreetName').val(obj[0].StreetName);
                           $('#JobElectricians_StreetTypeID').val(obj[0].StreetTypeID);
                           $('#JobElectricians_Town').val(obj[0].Town);
                           $('#JobElectricians_State').val(obj[0].State);
                           $('#JobElectricians_PostCode').val(obj[0].PostCode);

                           if (obj[0].Signature != "") {
                               var SignName = obj[0].Signature;
                               var guid = obj[0].UserID;

                               var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
                               var SRC = imagePath + "/" + SignName;
                               //$('#imgSign').attr('src' , SRC)
                               var SignName = $('#imgSign').attr('src');

                               SRCSign = SRC;

                               $('#imgSign').attr('class', SignName);

                               $('#imgSignature').show();

                               //$('<input type="hidden">').attr({
                               //    name: 'Signature',
                               //    value: obj[0].Signature,
                               //    id: obj[0].Signature,
                               //}).appendTo('form');

                               $("input[type='hidden'][name='Signature']").val(obj[0].Signature);

                               $('<input type="hidden">').attr({
                                   name: 'ProfileSignature',
                                   value: obj[0].Signature,
                                   id: obj[0].Signature,
                               }).appendTo('form');

                               $('<input type="hidden">').attr({
                                   name: 'ProfileSignatureID',
                                   value: obj[0].UserID,
                                   id: obj[0].UserID,
                               }).appendTo('form');

                           } else {
                               $('#imgSign').attr('class', "");
                               $('#imgSign').removeAttr('src');
                               $('#imgSignature').hide();
                           }

                           ElectrianAddressJson = [];
                           var address;
                           var UnitTypeId = $("#JobElectricians_UnitTypeID").find("option:selected").text();
                           var UnitNumber = $("#JobElectricians_UnitNumber").val();
                           var StreetNumber = $("#JobElectricians_StreetNumber").val();
                           var StreetName = $("#JobElectricians_StreetName").val();
                           var StreetTypeId = $("#JobElectricians_StreetTypeID").find("option:selected").text();
                           var PostalAddressID = $("#JobElectricians_PostalAddressID").find("option:selected").text();
                           var PostalDeliveryNumber = $("#JobElectricians_PostalDeliveryNumber").val();
                           var Town = $("#JobElectricians_Town").val();
                           var State = $("#JobElectricians_State").val();
                           var PostCode = $("#JobElectricians_PostCode").val();
                           //bansi1
                           var companyName = $('#JobElectricians_CompanyName').val();
                           var firstName = $('#JobElectricians_FirstName').val();
                           var lastName = $('#JobElectricians_LastName').val();
                           var email = $('#JobElectricians_Email').val();
                           var phone = $('#JobElectricians_Phone').val();
                           var mobile = $('#JobElectricians_Mobile').val();
                           var licenseNumber = $('#JobElectricians_LicenseNumber').val();

                           if ($("#JobElectricians_AddressID").val() == 1) {
                               if (UnitNumber != "") {
                                   address = UnitTypeId + ' ' + UnitNumber + "/" + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                                   address = address.replace("Select", "");
                               } else {
                                   address = UnitTypeId + ' ' + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                                   address = address.replace("Select", "");
                               }
                               ElectrianAddressJson.push({ PostalAddressType: $("#JobElectricians_AddressID").val(), UnitType: $("#JobElectricians_UnitTypeID").val(), UnitNumber: $("#JobElectricians_UnitNumber").val(), StreetNumber: $("#JobElectricians_StreetNumber").val(), StreetName: $("#JobElectricians_StreetName").val(), StreetType: $("#JobElectricians_StreetTypeID").val(), Town: $("#JobElectricians_Town").val(), State: $("#JobElectricians_State").val(), PostCode: $("#JobElectricians_PostCode").val(), CompanyName: companyName, FirstName: firstName, LastName: lastName, Email: email, Phone: phone, Mobile: mobile, LicenseNumber: licenseNumber });

                           } else {
                               address = PostalAddressID + ' ' + PostalDeliveryNumber + ', ' + Town + ' ' + State + ' ' + PostCode;
                               ElectrianAddressJson.push({ PostalAddressType: $("#JobElectricians_AddressID").val(), PostalAddressID: $("#JobElectricians_PostalAddressID").val(), PostalDeliveryNumber: $("#JobElectricians_PostalDeliveryNumber").val(), Town: $("#JobElectricians_Town").val(), State: $("#JobElectricians_State").val(), PostCode: $("#JobElectricians_PostCode").val(), CompanyName: companyName, FirstName: firstName, LastName: lastName, Email: email, Phone: phone, Mobile: mobile, LicenseNumber: licenseNumber })
                           }

                           //$("#txtBasicAddress").val($('#JobElectricians_FirstName').val() + ' ' + $('#JobElectricians_LastName').val());
                           // $("#txtBasicAddress").val("");
                           $('#spantxtBasicAddress').hide();

                       }
                   }
               });
    });


    $('#JobInstallerDetails_ElectricianID').change(function () {
        if ($(this).val() > 0) {
            var id = $('#JobInstallerDetails_ElectricianID').val();
            var jobType = $("#BasicDetails_JobType").val();
            $.ajax(
                   {
                       url: urlGetElectricianDetailbySolarcompany,
                       data: { Id: id, JobType: jobType },
                       contentType: 'application/json',
                       method: 'get',
                       success: function (data) {
                           if (data != false) {
                               var obj = JSON.parse(data);                           
                               if (obj[0].SESignature != "") {
                                   var SignName = obj[0].SESignature;
                                   var guid = ProjectSession_LoggedInUserId;
                                   var proofDocumentURL = UploadedDocumentPath;
                                   var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
                                   var SRC = imagePath + "/" + SignName;                                  
                                   var SignName = $('#imgSignSWHInstall').attr('src');
                                   SRCSignSWHInstall = SRC;
                                   $('#imgSignSWHInstall').attr('class', SignName);
                                   $('#imgSWHSignatureInstaller').show();
                                   $("input[type='hidden'][name='Signature']").val(obj[0].SESignature);

                               } else {
                                   $('#imgSignSWHInstall').attr('class', "");
                                   $('#imgSignSWHInstall').removeAttr('src');
                                   $('#imgSWHSignatureInstaller').hide();
                               }



                               $('#JobInstallerDetails_FirstName').val(obj[0].FirstName);
                               $('#JobInstallerDetails_Surname').val(obj[0].LastName);
                               $('#JobInstallerDetails_Email').val(obj[0].Email);
                               $('#JobInstallerDetails_Phone').val(obj[0].Phone);
                               $('#JobInstallerDetails_Mobile').val(obj[0].Mobile);
                               $('#JobInstallerDetails_LicenseNumber').val(obj[0].ElectricalContractorsLicenseNumber);
                               $("#JobInstallerDetails_LicenseNumber").attr("readonly", "readonly");
                               $('#JobInstallerDetails_LicenseNumber').attr('onblur', '')

                               var ispostal = 'False';
                               if (obj[0].IsPostalAddress == 'False') {
                                   ispostal = '1'
                                   $('.InstallerPDA').hide();
                                   $('.InstallerDPA').show();
                               } else {
                                   ispostal = '2'
                                   $('.InstallerDPA').hide();
                                   $('.InstallerPDA').show();
                               }
                               $('#JobInstallerDetails_AddressID').val(ispostal);
                               $('#JobInstallerDetails_UnitTypeID').val(obj[0].UnitTypeID);
                               $('#JobInstallerDetails_UnitNumber').val(obj[0].UnitNumber);
                               $('#JobInstallerDetails_StreetNumber').val(obj[0].StreetNumber);
                               $('#JobInstallerDetails_StreetName').val(obj[0].StreetName);
                               $('#JobInstallerDetails_StreetTypeID').val(obj[0].StreetTypeID);
                               $('#JobInstallerDetails_Town').val(obj[0].Town);
                               $('#JobInstallerDetails_State').val(obj[0].State);
                               $('#JobInstallerDetails_PostCode').val(obj[0].PostCode);
                               $("#JobInstallerDetails_PostalDeliveryNumber").val(obj[0].PostalDeliveryNumber);
                               $('#JobInstallerDetails_PostalAddressID').val(obj[0].PostalAddressID);

                               ElectrianAddressJson = [];
                               var address;
                               var UnitTypeId = $("#JobInstallerDetails").find("option:selected").text();
                               var UnitNumber = $("#JobInstallerDetails.UnitNumber").val();
                               var StreetNumber = $("#JobInstallerDetails_StreetNumber").val();
                               var StreetName = $("#JobInstallerDetails_StreetName").val();
                               var StreetTypeId = $("#JobInstallerDetails_StreetTypeID").find("option:selected").text();
                               var Town = $("#JobInstallerDetails_Town").val();
                               var State = $("#JobInstallerDetails_State").val();
                               var PostCode = $("#JobInstallerDetails_PostCode").val();
                               var firstName = $('#JobInstallerDetails_FirstName').val();
                               var lastName = $('#JobInstallerDetails_LastName').val();
                               var email = $('#JobInstallerDetails_Email').val();
                               var phone = $('#JobInstallerDetails_Phone').val();
                               var mobile = $('#JobInstallerDetails_Mobile').val();

                               if ($("#JobElectricians_AddressID").val() == 1) {
                                   if (UnitNumber != "") {
                                       address = UnitTypeId + ' ' + UnitNumber + "/" + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                                       address = address.replace("Select", "");
                                   } else {
                                       address = UnitTypeId + ' ' + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                                       address = address.replace("Select", "");
                                   }
                                   ElectrianAddressJson.push({
                                       PostalAddressType: $("#JobInstallerDetails_AddressID").val(), UnitType: $("#JobInstallerDetails.UnitTypeID").val(), UnitNumber: $("#JobInstallerDetails.UnitNumber").val(),
                                       StreetNumber: $("#JobInstallerDetails_StreetNumber").val(), StreetName: $("#JobInstallerDetails_StreetName").val(), StreetType: $("#JobInstallerDetails_StreetTypeID").val(), Town: $("#JobInstallerDetails_Town").val(),
                                       State: $("#JobInstallerDetails_State").val(), PostCode: $("#JobInstallerDetails_PostCode").val(), FirstName: firstName, LastName: lastName, Email: email, Phone: phone, Mobile: mobile
                                   });

                               } else {
                                   address = Town + ' ' + State + ' ' + PostCode;
                                   ElectrianAddressJson.push({
                                       PostalAddressType: $("#JobInstallerDetails_AddressID").val(),
                                       Town: $("#JobInstallerDetails_Town").val(),
                                       State: $("#JobInstallerDetails_State").val(),
                                       PostCode: $("#JobInstallerDetails_PostCode").val(),
                                       FirstName: firstName, LastName: lastName, Email: email, Phone: phone, Mobile: mobile
                                   })
                               }

                               $("#txtInstallerName").val($('#JobInstallerDetails_FirstName').val() + ' ' + $('#JobInstallerDetails_Surname').val());
                               $('#spantxtInstallerNames').hide();

                           }
                       }
                   });
        }
        else
            clearPopupInstaller();
    });

    $('#JobElectricians_ElectricianID').change(function () {
        $('#JobElectricians_InstallerID').val("");
        var id = $('#JobElectricians_ElectricianID').val();
        $.ajax(
               {
                   url: urlGetElectricianDetailbySolarcompany,
                   data: { Id: id },
                   contentType: 'application/json',
                   method: 'get',
                   success: function (data) {
                       if (data != false) {
                           var obj = JSON.parse(data);
                           $('#JobElectricians_CompanyName').val(obj[0].CompanyName);
                           $('#JobElectricians_FirstName').val(obj[0].FirstName);
                           $('#JobElectricians_LastName').val(obj[0].LastName);
                           $('#JobElectricians_Email').val(obj[0].Email);
                           $('#JobElectricians_Phone').val(obj[0].Phone);
                           $('#JobElectricians_Mobile').val(obj[0].Mobile);
                           $('#JobElectricians_LicenseNumber').val(obj[0].ElectricalContractorsLicenseNumber);

                           var ispostal = 'False';
                           if (obj[0].IsPostalAddress == 'False') {
                               ispostal = '1'
                               $('.ElectriciansPDA').hide();
                               $('.ElectriciansDPA').show();
                           } else {
                               ispostal = '2'
                               $('.ElectriciansDPA').hide();
                               $('.ElectriciansPDA').show();
                           }
                           $('#JobElectricians_AddressID').val(ispostal);
                           $('#JobElectricians_PostalAddressID').val(obj[0].PostalAddressID);
                           $('#JobElectricians_PostalDeliveryNumber').val(obj[0].PostalDeliveryNumber);
                           $('#JobElectricians_UnitTypeID').val(obj[0].UnitTypeID);
                           $('#JobElectricians_UnitNumber').val(obj[0].UnitNumber);
                           $('#JobElectricians_StreetNumber').val(obj[0].StreetNumber);
                           $('#JobElectricians_StreetName').val(obj[0].StreetName);
                           $('#JobElectricians_StreetTypeID').val(obj[0].StreetTypeID);
                           $('#JobElectricians_Town').val(obj[0].Town);
                           $('#JobElectricians_State').val(obj[0].State);
                           $('#JobElectricians_PostCode').val(obj[0].PostCode);

                           //changes for signature.
                           if (obj[0].Signature != "") {
                               var SignName = obj[0].Signature;
                               var guid = obj[0].JobElectricianID;
                               var imagePath = proofDocumentURL + "JobDocuments" + "/" + guid;
                               var SRC = imagePath + "/" + SignName;
                               //$('#imgSign').attr('src' , SRC)
                               var SignName = $('#imgSign').attr('src');

                               SRCSign = SRC;

                               $('#imgSign').attr('class', SignName);

                               $('#imgSignature').show();
                           } else {
                               $('#imgSign').attr('class', "");
                               $('#imgSign').removeAttr('src');
                               $('#imgSignature').hide();
                           }

                           ElectrianAddressJson = [];
                           var address;
                           var UnitTypeId = $("#JobElectricians_UnitTypeID").find("option:selected").text();
                           var UnitNumber = $("#JobElectricians_UnitNumber").val();
                           var StreetNumber = $("#JobElectricians_StreetNumber").val();
                           var StreetName = $("#JobElectricians_StreetName").val();
                           var StreetTypeId = $("#JobElectricians_StreetTypeID").find("option:selected").text();
                           var PostalAddressID = $("#JobElectricians_PostalAddressID").find("option:selected").text();
                           var PostalDeliveryNumber = $("#JobElectricians_PostalDeliveryNumber").val();
                           var Town = $("#JobElectricians_Town").val();
                           var State = $("#JobElectricians_State").val();
                           var PostCode = $("#JobElectricians_PostCode").val();
                           //bansi2
                           var companyName = $('#JobElectricians_CompanyName').val();
                           var firstName = $('#JobElectricians_FirstName').val();
                           var lastName = $('#JobElectricians_LastName').val();
                           var email = $('#JobElectricians_Email').val();
                           var phone = $('#JobElectricians_Phone').val();
                           var mobile = $('#JobElectricians_Mobile').val();
                           var licenseNumber = $('#JobElectricians_LicenseNumber').val();

                           if ($("#JobElectricians_AddressID").val() == 1) {
                               if (UnitNumber != "") {
                                   address = UnitTypeId + ' ' + UnitNumber + "/" + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                                   address = address.replace("Select", "");
                               } else {
                                   address = UnitTypeId + ' ' + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                                   address = address.replace("Select", "");
                               }
                               ElectrianAddressJson.push({ PostalAddressType: $("#JobElectricians_AddressID").val(), UnitType: $("#JobElectricians_UnitTypeID").val(), UnitNumber: $("#JobElectricians_UnitNumber").val(), StreetNumber: $("#JobElectricians_StreetNumber").val(), StreetName: $("#JobElectricians_StreetName").val(), StreetType: $("#JobElectricians_StreetTypeID").val(), Town: $("#JobElectricians_Town").val(), State: $("#JobElectricians_State").val(), PostCode: $("#JobElectricians_PostCode").val(), CompanyName: companyName, FirstName: firstName, LastName: lastName, Email: email, Phone: phone, Mobile: mobile, LicenseNumber: licenseNumber });

                           } else {
                               address = PostalAddressID + ' ' + PostalDeliveryNumber + ', ' + Town + ' ' + State + ' ' + PostCode;
                               ElectrianAddressJson.push({ PostalAddressType: $("#JobElectricians_AddressID").val(), PostalAddressID: $("#JobElectricians_PostalAddressID").val(), PostalDeliveryNumber: $("#JobElectricians_PostalDeliveryNumber").val(), Town: $("#JobElectricians_Town").val(), State: $("#JobElectricians_State").val(), PostCode: $("#JobElectricians_PostCode").val(), CompanyName: companyName, FirstName: firstName, LastName: lastName, Email: email, Phone: phone, Mobile: mobile, LicenseNumber: licenseNumber })
                           }

                           $("#txtBasicAddress").val($('#JobElectricians_FirstName').val() + ' ' + $('#JobElectricians_LastName').val());
                           $('#spantxtBasicAddress').hide();

                       }
                   }
               });
    });

    $("#btnCollectSignature").click(function () {
        //Email configuration not required

        var email;
        var message;
        var JobType = $("#BasicDetails_JobType").val();
        var IsOwner;
        var FirstName = '';
        var LastName = '';
        if (JobType == 1) {
            if ($('#drpSendMail').val() == "2") {

                email = $("#BasicDetails_InstallerID").val();
                message = "Installer  is required for it."
                IsOwner = 'No';
            }
            else {
                email = OwnerEmailFromJson;
                message = "Owner Email is required for it."
                IsOwner = 'Yes';
                FirstName = OwnerFirstNameFromJson;
                LastName = OwnerLastNameFromJson;

            }

            if (email != "" && email != null && email != undefined) {


                var mailUrl = '<a href=' + maillink + '>' + maillink + '</a>';
                var mailTo = email;
                $.ajax(
                       {
                           url: urlMailForCollectSignature,
                           data: { MailUrl: mailUrl, MailTo: mailTo, JobType: JobType, IsOwner: IsOwner, FirstName: FirstName, LastName: LastName },
                           contentType: 'application/json',
                           method: 'get',
                           success: function (data) {
                               if (data == true) {
                                   $(".alert").hide();
                                   $("#successMsgRegion").html(closeButton + "Mail has been send successfully.");
                                   $("#successMsgRegion").show();
                                   return false;
                               }
                               else {
                                   $(".alert").hide();
                                   $("#errorMsgRegion").html(closeButton + "Mail has not been sent.");
                                   $("#errorMsgRegion").show();
                               }
                           }
                       });
                //}
            }
            else {
                $('#spantxtCollectDignature').show().text(message).fadeOut(3000);
            }
        }
        else {
            if ($('#drpSendMail').val() == "2") {

                email = InstallerEmailFromJson;
                message = "Installer Email is required for it."
                IsOwner = 'No';
                FirstName = InstallerFirstNameFromJson;
                LastName = InstallerLastNameFromJson;
            }
            else {
                email = OwnerEmailFromJson;
                message = "Owner Email is required for it."
                IsOwner = 'Yes';
                FirstName = OwnerFirstNameFromJson;
                LastName = OwnerLastNameFromJson;
            }

            if (email != "" && email != null && email != undefined) {
                var mailUrl = '<a href=' + maillink + '>' + maillink + '</a>';
                var mailTo = email;
                $.ajax(
                       {
                           url: urlMailForCollectSignature,
                           data: { MailUrl: mailUrl, MailTo: mailTo, JobType: JobType, IsOwner: IsOwner, FirstName: FirstName, LastName: LastName },
                           contentType: 'application/json',
                           method: 'get',
                           success: function (data) {
                               if (data == true) {
                                   $(".alert").hide();
                                   $("#successMsgRegion").html(closeButton + "Mail has been send successfully.");
                                   $("#successMsgRegion").show();
                                   //$("#successMsgRegion").fadeOut(3000);
                                   return false;

                               }
                           }
                       });
            }
            else {
                $('#spantxtCollectDignature').show().text(message).fadeOut(3000);
            }
        }
        //}
        //else
        //{
        //    //Email configuration not required
        //    EmailAccountConfigureErrorMessage();
        //    return false;
        //}
    });

    $("#loading-image").hide();

});
//end Document ready.
function validateInstallation() {
    addressInstallationValidationRules();
    if ($('#JobInstallationAddress').valid()) {
        InstallationJson = [];
        var address;
        var UnitTypeId = $("#JobInstallationDetails_UnitTypeID").find("option:selected").text();
        var UnitNumber = $("#JobInstallationDetails_UnitNumber").val();
        var StreetNumber = $("#JobInstallationDetails_StreetNumber").val();
        var StreetName = $("#JobInstallationDetails_StreetName").val();
        var StreetTypeId = $("#JobInstallationDetails_StreetTypeID").find("option:selected").text();
        var PostalAddressID = $("#JobInstallationDetails_PostalAddressID").find("option:selected").text();
        var PostalDeliveryNumber = $("#JobInstallationDetails_PostalDeliveryNumber").val();
        var Town = $("#JobInstallationDetails_Town").val();
        var State = $("#JobInstallationDetails_State").val();
        var PostCode = $("#JobInstallationDetails_PostCode").val();
        if ($("#JobInstallationDetails_AddressID").val() == 1) {
            if (UnitNumber != "") {
                address = UnitTypeId + ' ' + UnitNumber + "/" + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                address = address.replace("Select", "");
            } else {
                address = UnitTypeId + ' ' + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                address = address.replace("Select", "");
            }
            InstallationJson.push({ PostalAddressType: $("#JobInstallationDetails_AddressID").val(), UnitType: $("#JobInstallationDetails_UnitTypeID").val(), UnitNumber: $("#JobInstallationDetails_UnitNumber").val(), StreetNumber: $("#JobInstallationDetails_StreetNumber").val(), StreetName: $("#JobInstallationDetails_StreetName").val(), StreetType: $("#JobInstallationDetails_StreetTypeID").val(), Town: $("#JobInstallationDetails_Town").val(), State: $("#JobInstallationDetails_State").val(), PostCode: $("#JobInstallationDetails_PostCode").val() });
        } else {
            address = PostalAddressID + ' ' + PostalDeliveryNumber + ', ' + Town + ' ' + State + ' ' + PostCode;
            InstallationJson.push({ PostalAddressType: $("#JobInstallationDetails_AddressID").val(), PostalAddressID: $("#JobInstallationDetails_PostalAddressID").val(), PostalDeliveryNumber: $("#JobInstallationDetails_PostalDeliveryNumber").val(), Town: $("#JobInstallationDetails_Town").val(), State: $("#JobInstallationDetails_State").val(), PostCode: $("#JobInstallationDetails_PostCode").val() });

        }
        InstallationPostcodeFromjson = $("#JobInstallationDetails_PostCode").val();
        $("#JobInstallationDetails_AddressDisplay").val(address);
        $("#txtAddress").val(address);
        $('#spantxtAddress').hide();
        $('#popupAddress').modal('toggle');
    }
}

function validateBasicAddress() {
    addressElectricianValidationRules();
    if ($('#JobElectricians').valid()) {
        ElectrianAddressJson = [];
        var address;
        var UnitTypeId = $("#JobElectricians_UnitTypeID").find("option:selected").text();
        var UnitNumber = $("#JobElectricians_UnitNumber").val();
        var StreetNumber = $("#JobElectricians_StreetNumber").val();
        var StreetName = $("#JobElectricians_StreetName").val();
        var StreetTypeId = $("#JobElectricians_StreetTypeID").find("option:selected").text();
        var PostalAddressID = $("#JobElectricians_PostalAddressID").find("option:selected").text();
        var PostalDeliveryNumber = $("#JobElectricians_PostalDeliveryNumber").val();
        var Town = $("#JobElectricians_Town").val();
        var State = $("#JobElectricians_State").val();
        var PostCode = $("#JobElectricians_PostCode").val();
        //bansi4
        var companyName = $('#JobElectricians_CompanyName').val();
        var firstName = $('#JobElectricians_FirstName').val();
        var lastName = $('#JobElectricians_LastName').val();
        var email = $('#JobElectricians_Email').val();
        var phone = $('#JobElectricians_Phone').val();
        var mobile = $('#JobElectricians_Mobile').val();
        var licenseNumber = $('#JobElectricians_LicenseNumber').val();

        if ($("#JobElectricians_AddressID").val() == 1) {
            if (UnitNumber != "") {
                address = UnitTypeId + ' ' + UnitNumber + "/" + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                address = address.replace("Select", "");
            } else {
                address = UnitTypeId + ' ' + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                address = address.replace("Select", "");
            }
            ElectrianAddressJson.push({ PostalAddressType: $("#JobElectricians_AddressID").val(), UnitType: $("#JobElectricians_UnitTypeID").val(), UnitNumber: $("#JobElectricians_UnitNumber").val(), StreetNumber: $("#JobElectricians_StreetNumber").val(), StreetName: $("#JobElectricians_StreetName").val(), StreetType: $("#JobElectricians_StreetTypeID").val(), Town: $("#JobElectricians_Town").val(), State: $("#JobElectricians_State").val(), PostCode: $("#JobElectricians_PostCode").val(), CompanyName: companyName, FirstName: firstName, LastName: lastName, Email: email, Phone: phone, Mobile: mobile, LicenseNumber: licenseNumber });

        } else {
            address = PostalAddressID + ' ' + PostalDeliveryNumber + ', ' + Town + ' ' + State + ' ' + PostCode;
            ElectrianAddressJson.push({ PostalAddressType: $("#JobElectricians_AddressID").val(), PostalAddressID: $("#JobElectricians_PostalAddressID").val(), PostalDeliveryNumber: $("#JobElectricians_PostalDeliveryNumber").val(), Town: $("#JobElectricians_Town").val(), State: $("#JobElectricians_State").val(), PostCode: $("#JobElectricians_PostCode").val(), CompanyName: companyName, FirstName: firstName, LastName: lastName, Email: email, Phone: phone, Mobile: mobile, LicenseNumber: licenseNumber })
        }
        $("#txtBasicAddress").val($('#JobElectricians_FirstName').val() + ' ' + $('#JobElectricians_LastName').val());
        $('#spantxtBasicAddress').hide();
        $('#popupBasicAddress').modal('toggle');

        electricianSignSRC = SRCSign;
    }
}
//todo

function validateSoldBy() {
    debugger;
    if ($('#BasicDetails_JobSoldByText').val() != '') {
        var soldBy = $('#BasicDetails_JobSoldByText').val();
        var soldbyDate = $('#BasicDetails_strSoldByDate').val();
        $('#BasicDetails_SoldBy').val(soldBy);
        $('#BasicDetails_SoldByDate').val(soldbyDate);
        jsonvarSoldByDate = soldbyDate;
        jsonvarJobSoldBy = $('#BasicDetails_JobSoldByText').val();

        $('#popupSoldBy').modal('toggle');
    }
    else {
        $('#spanSoldBy').show().fadeOut(5000);
    }

}
function validateOwner() {
    addressOwnerValidationRules();
    if ($('#JobOwner').valid()) {
        OwnerAddressJson = [];
        var clientname;
        if ($("#JobOwnerDetails_OwnerType").val() == 'Government body' || $("#JobOwnerDetails_OwnerType").val() == 'Corporate body' || $("#JobOwnerDetails_OwnerType").val() == 'Trustee') {
            clientname = $("#JobOwnerDetails_CompanyName").val();
        } else {
            var FirstName = $("#JobOwnerDetails_FirstName").val();
            var LastName = $("#JobOwnerDetails_LastName").val();
            clientname = FirstName + ' ' + LastName;
        }

        $('#spantxtClientNames').hide();
        $("#txtClientName").val(clientname);
        $('#popupOwner').modal('toggle');
        if ($("#JobOwnerDetails_AddressID").val() == 1) {
            OwnerAddressJson.push({ OwnerType: $("#JobOwnerDetails_OwnerType").val(), CompanyName: $("#JobOwnerDetails_CompanyName").val(), Phone: $('#JobOwnerDetails_Phone').val(), Mobile: $('#JobOwnerDetails_Mobile').val(), FirstName: $('#JobOwnerDetails_FirstName').val(), LastName: $('#JobOwnerDetails_LastName').val(), Email: $('#JobOwnerDetails_Email').val(), PostalAddressType: $("#JobOwnerDetails_AddressID").val(), UnitType: $("#JobOwnerDetails_UnitTypeID").val(), UnitNumber: $("#JobOwnerDetails_UnitNumber").val(), StreetNumber: $("#JobOwnerDetails_StreetNumber").val(), StreetName: $("#JobOwnerDetails_StreetName").val(), StreetType: $("#JobOwnerDetails_StreetTypeID").val(), Town: $("#JobOwnerDetails_Town").val(), State: $("#JobOwnerDetails_State").val(), PostCode: $("#JobOwnerDetails_PostCode").val() });
        } else {
            OwnerAddressJson.push({ OwnerType: $("#JobOwnerDetails_OwnerType").val(), CompanyName: $("#JobOwnerDetails_CompanyName").val(), Phone: $('#JobOwnerDetails_Phone').val(), Mobile: $('#JobOwnerDetails_Mobile').val(), FirstName: $('#JobOwnerDetails_FirstName').val(), LastName: $('#JobOwnerDetails_LastName').val(), Email: $('#JobOwnerDetails_Email').val(), PostalAddressType: $("#JobOwnerDetails_AddressID").val(), PostalAddressID: $("#JobOwnerDetails_PostalAddressID").val(), PostalDeliveryNumber: $("#JobOwnerDetails_PostalDeliveryNumber").val(), Town: $("#JobOwnerDetails_Town").val(), State: $("#JobOwnerDetails_State").val(), PostCode: $("#JobOwnerDetails_PostCode").val() });
        }
        OwnerPostcodeFromjson = $("#JobOwnerDetails_PostCode").val();
        OwnerEmailFromJson = $('#JobOwnerDetails_Email').val();
        OwnerFirstNameFromJson = $("#JobOwnerDetails_FirstName").val();
        OwnerLastNameFromJson = $("#JobOwnerDetails_LastName").val();
    }
}

function validateInstaller() {
    addressInstallerValidationRules();
    if ($('#Installer').valid()) {
        InstallerJson = [];
        var Installername;
        var FirstName = $("#JobInstallerDetails_FirstName").val();
        var Surname = $("#JobInstallerDetails_Surname").val();
        Installername = FirstName + ' ' + Surname;
        $('#spantxtClientNames').hide();
        $("#txtInstallerName").val(Installername);
        $('#popupInstaller').modal('toggle');
        if ($("#JobInstallerDetails_AddressID").val() == 1) {
            InstallerJson.push({ Phone: $('#JobInstallerDetails_Phone').val(), Mobile: $('#JobInstallerDetails_Mobile').val(), FirstName: $('#JobInstallerDetails_FirstName').val(), Surname: $('#JobInstallerDetails_Surname').val(), Email: $('#JobInstallerDetails_Email').val(), PostalAddressType: $("#JobInstallerDetails_AddressID").val(), UnitType: $("#JobInstallerDetails_UnitTypeID").val(), UnitNumber: $("#JobInstallerDetails_UnitNumber").val(), StreetNumber: $("#JobInstallerDetails_StreetNumber").val(), StreetName: $("#JobInstallerDetails_StreetName").val(), StreetType: $("#JobInstallerDetails_StreetTypeID").val(), Town: $("#JobInstallerDetails_Town").val(), State: $("#JobInstallerDetails_State").val(), PostCode: $("#JobInstallerDetails_PostCode").val(), SolarCompanyId: company_Id, SESignature: $("input[type='hidden'][name='Signature']").val(), ElectricianID: $("#JobInstallerDetails_ElectricianID").val() });
        } else {
            InstallerJson.push({ Phone: $('#JobInstallerDetails_Phone').val(), Mobile: $('#JobInstallerDetails_Mobile').val(), FirstName: $('#JobInstallerDetails_FirstName').val(), Surname: $('#JobInstallerDetails_Surname').val(), Email: $('#JobInstallerDetails_Email').val(), PostalAddressType: $("#JobInstallerDetails_AddressID").val(), PostalAddressID: $("#JobInstallerDetails_PostalAddressID").val(), PostalDeliveryNumber: $("#JobInstallerDetails_PostalDeliveryNumber").val(), Town: $("#JobInstallerDetails_Town").val(), State: $("#JobInstallerDetails_State").val(), PostCode: $("#JobInstallerDetails_PostCode").val(), SolarCompanyId: company_Id, SESignature: $("input[type='hidden'][name='Signature']").val(), ElectricianID: $("#JobInstallerDetails_ElectricianID").val() });

        }
        InstallerEmailFromJson = $('#JobInstallerDetails_Email').val();
        InstallerFirstNameFromJson = $("#JobInstallerDetails_FirstName").val();
        InstallerLastNameFromJson = $("#JobInstallerDetails_Surname").val();

        //JobInstallerDetailsSWH_SESignature = SRCSignSWHInstall;
        //installerSignSRC = SRCSignSWHInstall;

        $("#JobInstallerDetails_SWHInstallerDesignerId").val($("#JobInstallerDetails_ElectricianID").val());
        $("#JobInstallerDetails_SESignature").val($("input[type='hidden'][name='Signature']").val());
    }
}

function DeletePanelFromGrid(trId) {    
    if (confirm('Are you sure you want to delete this panel?')) {
        PanelXml = $.grep(PanelXml, function (e, i) {
            //if (e.ID == trId) {
            if (e.ID.replace("action", "").trim() == trId) {
                $("#JobSystemDetails_NoOfPanel").val(parseInt($("#JobSystemDetails_NoOfPanel").val()) - parseInt(e.Count));
                document.getElementById("tblPanel").deleteRow(i);
                return false;
            } else { return true; }
        });
        if ($('#Paneldatatable tr').length == 1) {
            $('#gridPanel').hide();
        }
    }
}

function DeleteInverterFromGrid(trId) {
    if (confirm('Are you sure you want to delete this Inverter ?')) {
        InverterXml = $.grep(InverterXml, function (e, i) {
            if (e.ID.replace("action", "").trim() == trId) {
                document.getElementById("tblInverter").deleteRow(i);
                return false;
            }
            else
                return true;
        });

        if ($('#Inverterdatatable tr').length == 1) {
            $('#gridInverter').hide();
        }
    }
}

function DeleteBatteryManufacturerFromGrid(trId) {
    if (confirm('Are you sure you want to delete this Battery manufacturer?')) {
        batteryXml = $.grep(batteryXml, function (e, i) {
            if (e.JobBatteryManufacturerId == trId) {
                document.getElementById("tblBatteryManufacturer").deleteRow(i);
                return false;
            }
            else
                return true;
        });

    }
}

function clearPopupInstallation() {
    $(".popupAddress").find('input[type=text]').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $(".popupAddress").find('.field-validation-error').attr('class', 'field-validation-valid');
    $("#JobInstallationDetails_UnitTypeID").val($("#JobInstallationDetails_UnitTypeID option:first").val());
    $("#JobInstallationDetails_StreetTypeID").val($("#JobInstallationDetails_StreetTypeID option:first").val());
    $("#JobInstallationDetails_PostalAddressID").val($("#JobInstallationDetails_PostalAddressID option:first").val());

}
function clearPopupBasicAddress() {
    $(".popupBasicAddress").find('input[type=text]').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $(".popupBasicAddress").find('.field-validation-error').attr('class', 'field-validation-valid');
    $("#JobElectricians_UnitTypeID").val($("#JobElectricians_UnitTypeID option:first").val());
    $("#JobElectricians_StreetTypeID").val($("#JobElectricians_StreetTypeID option:first").val());
    $("#JobElectricians_PostalAddressID").val($("#JobElectricians_PostalAddressID option:first").val());

}
function clearPopupOwner() {
    $(".popupOwner").find('input[type=text]').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $(".popupOwner").find('.field-validation-error').attr('class', 'field-validation-valid');
    $("#JobOwnerDetails_UnitTypeID").val($("#JobOwnerDetails_UnitTypeID option:first").val());
    $("#JobOwnerDetails_StreetTypeID").val($("#JobOwnerDetails_StreetTypeID option:first").val());
    $("#JobOwnerDetails_PostalAddressID").val($("#JobOwnerDetails_PostalAddressID option:first").val());
}
function clearPopupInstaller() {
    $(".popupInstaller").find('input[type=text]').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $(".popupInstaller").find('.field-validation-error').attr('class', 'field-validation-valid');
    $("#JobInstallerDetails_UnitTypeID").val($("#JobOwnerDetails_UnitTypeID option:first").val());
    $("#JobInstallerDetails_StreetTypeID").val($("#JobOwnerDetails_StreetTypeID option:first").val());
    $("#JobInstallerDetails_PostalAddressID").val($("#JobOwnerDetails_PostalAddressID option:first").val());
    $("#JobInstallerDetails_LicenseNumber").removeAttr("readonly");
    $('#JobInstallerDetails_LicenseNumber').attr('onblur', "checkExistSWHInstaller(this,'InstallerDesigner LicenseNumber')");
    $("#JobInstallerDetails_ElectricianID").val('');
    $("#imgSWHSignatureInstaller").hide();
}
function deleteImageJob(imageId) {
    var FolderName = Model_Guid;
    var fileDelete = $('#imgSign').attr('class');
    var OldEleSign = JobElectricians_Signature;
    if (confirm('Are you sure you want to delete this file ?')) {
        $.ajax(
                {
                    url: urlDeleteFileFromFolderAndElectrician,
                    data: { fileName: fileDelete, FolderName: FolderName, OldEleSign: OldEleSign },
                    contentType: 'application/json',
                    method: 'get',
                    success: function () {
                        var sign = $('#imgSign').attr('class');
                        $("[name='Signature']").each(function () {
                            $(this).remove();
                        });
                        $('#imgSign').removeAttr('src');
                        $('#imgSign').removeAttr('class');
                        $('#popupbox').modal('hide');
                        $("#imgSignature").hide();
                        $(".alert").hide();
                        //$("#successMsgRegion").html(closeButton + "File has been Deleted successfully.");
                        //$("#successMsgRegion").show();
                        //$("#successMsgRegion").fadeOut(3000);
                        return false;
                    }
                });
    }
}

function DeleteFileFromFolderAndElectrician(fileNames, guid, OldEleSign) {
    $.ajax(
        {
            url: urlDeleteFileFromFolderAndElectrician,
            data: { fileName: fileNames, FolderName: guid, OldEleSign: OldEleSign },
            contentType: 'application/json',
            method: 'get',
            success: function (data) {

            },
        });
}

function DeleteDocumentFolderOnCancel() {
    window.location.href = urlIndex;
}

function DeleteJobs() {
    selectedRows = [];
    jobDetail = {
        id: jobIDForScheduling,
        jobTitle: $('#BasicDetails_Title').val()
    }
    selectedRows.push(jobDetail);
    if (selectedRows != null && selectedRows.length > 0) {
        var result = confirm("Are you sure you want to delete selected jobs ?");
        if (result) {
            $.ajax({
                url: urlDeleteSelectedJobs,
                type: "POST",
                cache: false,
                async: true,
                data: JSON.stringify({ jobs: selectedRows }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.success) {
                        window.location.href = '/Job/Index';
                    }
                    else {
                        showErrorMessagesForStatus(data.JsonResponseObj.strErrors);
                    }
                },
            });
        }
    }
    else {
        alert('Please select any job for delete.');
    }

}

function CheckShowErrorMessagesForPopup() {
    var ReturnValue = true;
    //var ExplanatoryNotes = $('#JobSTCDetails_ExplanatoryNotes').val();
    var FailedCode = $("#JobSTCDetails_FailedAccreditationCode").val();
    var StatutoryDeclarations = $('#JobSTCDetails_StatutoryDeclarations').val();
    //var AdditionalCapacityNotes =$('#JobSTCDetails_AdditionalCapacityNotes').val();

    var InstallationAddress = $('#txtAddress').val();
    var OwnerDetails = $('#txtClientName').val();

    if ($("#JobInstallationDetails_InstallingNewPanel").val() != 'New' && $("#JobInstallationDetails_InstallingNewPanel").val() != "" && $("#JobInstallationDetails_Location").val() == "") {
        $("#spanInstallationLocation").show();
        ReturnValue = false;
    } else { $("#spanInstallationLocation").hide(); }

    if (JOBType == '1') {
        if ($("#JobSTCDetails_MultipleSGUAddress").val() == 'Yes' && $("#JobSTCDetails_Location").val() == "") {
            $("#spanSTCLocation").show();
            ReturnValue = false;
        } else { $("#spanSTCLocation").hide(); }
    }

    if (InstallationAddress.trim() == "" || InstallationAddress == null || InstallationAddress == undefined) {
        $('#spantxtAddress').show();
        ReturnValue = false;
    } else { $('#spantxtAddress').hide(); }

    if (OwnerDetails.trim() == "" || OwnerDetails == null || OwnerDetails == undefined) {
        $('#spantxtClientNames').show();
        ReturnValue = false;
    } else { $('#spantxtClientNames').hide(); }

    if ($('#JobSTCDetails_CertificateCreated').val() == "Yes") {
        if (FailedCode.trim() == "" || FailedCode == null || FailedCode == undefined) {
            $("#spanFailedAccreditationCode").show();
            ReturnValue = false;
        }
    }

    if ($("#JobSTCDetails_VolumetricCapacity").val() == "Yes") {

        if (StatutoryDeclarations == "Select" || StatutoryDeclarations == null || StatutoryDeclarations == undefined || StatutoryDeclarations.trim() == "") {
            $("#spanStatutoryDeclarations").show();
            ReturnValue = false;
        }
    }

    //if($('#BasicDetails_JobType').val() == 1)
    //{
    //    if($("#JobSTCDetails_InstallingCompleteUnit").val() == "No" && (AdditionalCapacityNotes.trim() == "" || AdditionalCapacityNotes == null || AdditionalCapacityNotes == undefined) )
    //    {
    //        $("#spanAdditionalCapacityNotes").show();
    //        $("#spanAdditionalCapacityNotesMinimum").hide();
    //        ReturnValue = false;
    //    } else if ($("#JobSTCDetails_AdditionalCapacityNotes").val().trim().length<8 && $("#JobSTCDetails_InstallingCompleteUnit").val() == "No") {
    //        //$("#spanAdditionalCapacityNotesMinimum").fadeIn(100)
    //        //$("#spanAdditionalCapacityNotesMinimum").fadeOut(7000)
    //        $("#spanAdditionalCapacityNotesMinimum").show();
    //        $("#spanAdditionalCapacityNotes").hide();
    //        ReturnValue = false;
    //    } else {
    //        $("#spanAdditionalCapacityNotes").hide();
    //        $("#spanAdditionalCapacityNotesMinimum").hide();
    //    }
    //}

    if ($('#BasicDetails_JobType').val() == 1) {
        if (parseFloat($("#JobSystemDetails_SystemSize").val()) <= 0) {
        }
    }
    return ReturnValue;
}
function SerialNumbersValidation() {
    //var serialNumbers = new Array();
    //serialNumbers = $('#JobSystemDetails_SerialNumbers').val().split(";");
    var isNumbersOverflow = false;
    var isDuplicateSerialNumber = false;

    //var panel = $('#JobSystemDetails_NoOfPanel').val();
    //if( panel =='' || panel == '0')
    //{
    //    return true;
    //}

    //if (serialNumbers.length == panel) {
    //    for(var i=0; i< serialNumbers.length; i++) {
    //        if (serialNumbers[i].length > 100) {
    //            isNumbersOverflow=true;
    //            break;
    //        }
    //    }

    //    for(var i=0; i< serialNumbers.length; i++) {
    //        if (serialNumbers[i].length == 0) {
    //            isNumbersNotEqualToPanel=true;
    //            break;
    //        }
    //    }
    //}
    //else {
    //    isNumbersNotEqualToPanel=true;
    //}
    //if ($("#JobSystemDetails_NoOfPanel").val()!="" && $("#JobSystemDetails_NoOfPanel").val()!="0") {

    if ($("#JobSystemDetails_SerialNumbers").val().trim() != "") {
        var text = $("#JobSystemDetails_SerialNumbers").val().trim();
        var lines = text.split("\n");
        lines = lines.filter(function (n) { return n.length > 0 });
        var count = lines.length;
        for (var i = 0; i < lines.length; i++) {
            if (lines[i].length > 100) {
                isNumbersOverflow = true;
                break;
            }
        }
        isDuplicateSerialNumber = hasDuplicates(lines);
    }

    if (isNumbersOverflow) {
        $('#spanSerialNumbers').show().text('Serial numbers\'s maximum length  is of 100 characters.');
    }
    else if (isDuplicateSerialNumber) {
        $('#spanSerialNumbers').show().text('Serial numbers should not be same.');
    }
    else {
        $('#spanSerialNumbers').hide();
    }

    if (isNumbersOverflow || isDuplicateSerialNumber) {
        return false;
    }
    else {
        return true;
    }
    //} else {
    //    return true;
    //}
}

function hasDuplicates(array) {
    var valuesSoFar = [];
    for (var i = 0; i < array.length; ++i) {
        var value = array[i];
        if (valuesSoFar.indexOf(value) !== -1) {
            return true;
        }
        valuesSoFar.push(value);
    }
    return false;
}

function editPanel(trId) {    
    $.each(PanelXml, function (i, e) {
        //if (e.ID == trId) {
        if (e.ID.replace("action", "").trim() == trId) {
            // setTimeout(function(){$("#JobPanelDetails_Model").val(e.Model)} , 10000);
            $("#JobPanelDetails_Brand").val(e.Brand);
            $("#JobPanelDetails_Model").prop("disabled", false);
            $("#JobPanelDetails_NoOfPanel").prop("disabled", false);
            FillDropDown('JobPanelDetails_Model', urlGetPanelModel + e.Brand + '&JobType=1', e.Model, true, null);
            $("#JobPanelDetails_NoOfPanel").val(e.Count);
            $("#btnEditPanel").show();
            $('#btnAddPanel').hide();
            $("#btnEditPanel").attr("name", trId);
        }
    })
}

function PanelEditAction(e) {    
    var panelBrand = $("#JobPanelDetails_Brand").val();
    var panelModel = $("#JobPanelDetails_Model").val();
    var noOfPanel = $("#JobPanelDetails_NoOfPanel").val();
    var TrId = e.name;
    var isDuplicate = false;
    if ((panelBrand != null && panelBrand != "") && (panelModel != null && panelModel != "") && (noOfPanel != null && noOfPanel != "" && noOfPanel != 0)) {
        //for (var i = 0; i < PanelXml.length; i++) {
        //    if(PanelXml[i].ID != TrId &&( PanelXml[i].Brand == panelBrand && PanelXml[i].Model == panelModel ))
        //    {
        //        isDuplicate=true;
        //        break;
        //    }
        //}

        //if (isDuplicate) {
        //    $("#spanJobPanelDetails_Model").show().text('Same panel is exist.').fadeOut(3000);
        //}
        //else {
        $.each(PanelXml, function (i, e) {
            if (e.ID.replace("action", "").trim() == TrId) {
                e.Brand = panelBrand;
                e.Model = panelModel;
                e.Count = noOfPanel;
            }
        });
        $('#tblPanel tr').each(function () {
            var panelBrand = $(this).find("td").eq(0).attr("class");
            var panelModel = $(this).find("td").eq(1).attr("class");
            var numberOfPanel = $(this).find("td").eq(2).attr("class");
            var trId = $(this).find("td").eq(3).attr("class");
            if (trId.replace("action", "").trim() == TrId) {
                $(this).find("td").eq(0).attr("class", $("#JobPanelDetails_Brand").val());
                $(this).find("td").eq(0).text($("#JobPanelDetails_Brand").val());
                $(this).find("td").eq(1).attr("class", $("#JobPanelDetails_Model").val());
                $(this).find("td").eq(1).text($("#JobPanelDetails_Model").val());
                $(this).find("td").eq(2).attr("class", $("#JobPanelDetails_NoOfPanel").val());
                $(this).find("td").eq(2).text($("#JobPanelDetails_NoOfPanel").val());

                $("#JobSystemDetails_NoOfPanel").val(parseInt($("#JobSystemDetails_NoOfPanel").val()) - numberOfPanel);
                $("#JobSystemDetails_NoOfPanel").val(parseInt($("#JobSystemDetails_NoOfPanel").val()) + parseInt($("#JobPanelDetails_NoOfPanel").val()));
            }

            $("#btnEditPanel").hide();
            $('#btnAddPanel').show();
        });
        //}
    }
    else {
        if (panelBrand == null || panelBrand.trim() == "") {
            $("#spanJobPanelDetails_Brand").show().fadeOut(3000);
        }

        if (panelModel == null || panelModel.trim() == "") {
            $("#spanJobPanelDetails_Model").show().text('Panel Model  is required..').fadeOut(3000);
        }

        if (noOfPanel == null || noOfPanel.trim() == "" || noOfPanel == 0) {
            $("#spanNoOfPanelValue").show().fadeOut(5000);
        }
    }

}


//Inverter
function editInverter(trId) {
    $.each(InverterXml, function (i, e) {
        if (e.ID.replace("action", "").trim() == trId) {
            // setTimeout(function(){$("#JobPanelDetails_Model").val(e.Model)} , 10000);
            $("#JobInverterDetails_Brand").val(e.Brand);
            $("#JobInverterDetails_Series").prop("disabled", false);
            FillDropDown('JobInverterDetails_Series', urlGetInverterModel + e.Brand, e.Series, true, null);
            $("#JobInverterDetails_Model").prop("disabled", false);
            FillDropDown('JobInverterDetails_Model', urlGetInverterSeries + e.Series + '&Manufacturer=' + e.Brand, e.Model, true, null);
            $("#btnEditInverter").show();
            $('#btnAddInverter').hide();
            $("#btnEditInverter").attr("name", trId);
        }
    })
}
function InverterEditAction(e) {
    var InverterBrand = $("#JobInverterDetails_Brand").val();
    var InverterModel = $("#JobInverterDetails_Model").val();
    var Series = $("#JobInverterDetails_Series").val();
    var TrId = e.name;

    var isDuplicate = false;

    if ((InverterBrand != null && InverterBrand != "") && (InverterModel != null && InverterModel != "") && (Series != null && Series != "")) {
        //for (var i = 0; i < InverterXml.length; i++) {
        //    if(InverterXml[i].ID != TrId &&( InverterXml[i].Brand == InverterBrand && InverterXml[i].Model == InverterModel && InverterXml[i].Series == Series ))
        //    {
        //        isDuplicate=true;
        //        break;
        //    }
        //}

        //if (isDuplicate) {
        //    $("#spanJobInverterDetails_Series").show().text('Same inverter is exist.').fadeOut(5000);
        //}
        //else {
        $.each(InverterXml, function (i, e) {
            if (e.ID.replace("action", "").trim() == TrId) {
                e.Brand = InverterBrand;
                e.Model = InverterModel;
                e.Series = Series;
            }
        });
        $('#tblInverter tr').each(function () {
            var inverterBrand = $(this).find("td").eq(0).attr("class");
            var series = $(this).find("td").eq(1).attr("class");
            var inverterModel = $(this).find("td").eq(2).attr("class");
            var trId = $(this).find("td").eq(3).attr("class");
            if (trId.replace("action", "").trim() == TrId) {
                $(this).find("td").eq(0).attr("class", InverterBrand);
                $(this).find("td").eq(0).text(InverterBrand);
                $(this).find("td").eq(1).attr("class", Series);
                $(this).find("td").eq(1).text(Series);
                $(this).find("td").eq(2).attr("class", InverterModel);
                $(this).find("td").eq(2).text(InverterModel);
            }

            $("#btnEditInverter").hide();
            $('#btnAddInverter').show();
        });
        //}
    }
    else {
        if (InverterBrand == null || InverterBrand.trim() == "")
            $("#spanJobInverterDetails_Brand").show().fadeOut(5000);

        if (InverterModel == null || InverterModel.trim() == "")
            $("#spanJobInverterDetails_Model").show().fadeOut(5000);

        if (Series == null || Series.trim() == "")
            $("#spanJobInverterDetails_Series").show().text("Inverter Series  is required.").fadeOut(5000);
    }
}

//Battery
function editBatteryManufacturer(trId) {
    $.each(batteryXml, function (i, e) {
        if (e.JobBatteryManufacturerId == trId) {

            $("#JobBatteryManufacturer_Manufacturer").val(e.Manufacturer);
            $("#JobBatteryManufacturer_ModelNumber").prop("disabled", false);
            FillDropDown('JobBatteryManufacturer_ModelNumber', urlGetBatteryModel + e.Manufacturer, e.ModelNumber, true, null);
            $("#btnEditBatteryManfacturer").show();
            $('#btnAddBatteryManfacturer').hide();
            $("#btnEditBatteryManfacturer").attr("name", trId);

        }
    });
}

function BatteryEditAction(e) {
    var batteryManufacturer = $("#JobBatteryManufacturer_Manufacturer").val();
    var batteryModel = $("#JobBatteryManufacturer_ModelNumber").val();
    var TrId = e.name;

    if ((batteryManufacturer != null && batteryManufacturer != "") && (batteryModel != null && batteryModel != "")) {
       
        $.each(batteryXml, function (i, e) {
            if (e.JobBatteryManufacturerId == TrId) {
                e.Manufacturer = batteryManufacturer;
                e.ModelNumber = batteryModel;
            }
        });

        $('#tblBatteryManufacturer tr').each(function () {
            var trId = $(this).find("td").eq(2).attr("class");
            if (trId == TrId) {
                $(this).find("td").eq(0).attr("class", batteryManufacturer);
                $(this).find("td").eq(0).text(batteryManufacturer);
                $(this).find("td").eq(1).attr("class", batteryModel);
                $(this).find("td").eq(1).text(batteryModel);
            }

            $("#btnEditBatteryManfacturer").hide();
            $('#btnAddBatteryManfacturer').show();
        });
        //}
    }
    else {
        if (batteryManufacturer == null || batteryManufacturer.trim() == "")
            $("#spanJobBatteryManufacturer").show();

        if (batteryModel == null || batteryModel.trim() == "")
            $("#spanJobBatteryModelNumber").show();
    }
}

function fillElectrianPopup() {
    if (ElectrianAddressJson.length > 0) {
        var AddressType = ElectrianAddressJson[0].PostalAddressType;
        $("#JobElectricians_AddressID").val(AddressType);
        if (AddressType == 1) {
            $("#JobElectricians_UnitTypeID").val(ElectrianAddressJson[0].UnitType);
            $("#JobElectricians_UnitNumber").val(ElectrianAddressJson[0].UnitNumber);
            $("#JobElectricians_StreetNumber").val(ElectrianAddressJson[0].StreetNumber);
            $("#JobElectricians_StreetName").val(ElectrianAddressJson[0].StreetName);
            $("#JobElectricians_StreetTypeID").val(ElectrianAddressJson[0].StreetType);
            $("#JobElectricians_PostalAddressID").val('');
            $("#JobElectricians_PostalDeliveryNumber").val('');
        }
        else {
            $("#JobElectricians_PostalAddressID").val(ElectrianAddressJson[0].PostalAddressID);
            $("#JobElectricians_PostalDeliveryNumber").val(ElectrianAddressJson[0].PostalDeliveryNumber);
            $("#JobElectricians_UnitTypeID").val("");
            $("#JobElectricians_UnitNumber").val("");
            $("#JobElectricians_StreetNumber").val("");
            $("#JobElectricians_StreetName").val("");
            $("#JobElectricians_StreetTypeID").val("");
        }

        $("#JobElectricians_Town").val(ElectrianAddressJson[0].Town);
        $("#JobElectricians_State").val(ElectrianAddressJson[0].State);
        $("#JobElectricians_PostCode").val(ElectrianAddressJson[0].PostCode);
        $('#JobElectricians_CompanyName').val(ElectrianAddressJson[0].CompanyName);
        $('#JobElectricians_FirstName').val(ElectrianAddressJson[0].FirstName);
        $('#JobElectricians_LastName').val(ElectrianAddressJson[0].LastName);
        $('#JobElectricians_Email').val(ElectrianAddressJson[0].Email);
        $('#JobElectricians_Phone').val(ElectrianAddressJson[0].Phone);
        $('#JobElectricians_Mobile').val(ElectrianAddressJson[0].Mobile);
        $('#JobElectricians_LicenseNumber').val(ElectrianAddressJson[0].LicenseNumber);
        //bansi5
    }

    else {
        $("#JobElectricians_AddressID").val(1);
        $("#JobElectricians_UnitTypeID").val("");
        $("#JobElectricians_UnitNumber").val("");
        $("#JobElectricians_StreetNumber").val("");
        $("#JobElectricians_StreetName").val("");
        $("#JobElectricians_StreetTypeID").val("");
        $("#JobElectricians_Town").val('');
        $("#JobElectricians_State").val('');
        $("#JobElectricians_PostCode").val('');
        $("#JobElectricians_PostalAddressID").val('');
        $("#JobElectricians_PostalDeliveryNumber").val('');
        $('#JobElectricians_CompanyName').val("");
        $('#JobElectricians_FirstName').val("");
        $('#JobElectricians_LastName').val("");
        $('#JobElectricians_Email').val("");
        $('#JobElectricians_Phone').val("");
        $('#JobElectricians_Mobile').val("");
        $('#JobElectricians_LicenseNumber').val("");
        $('.ElectriciansDPA').show();
        $('.ElectriciansPDA').hide();
        //bansi6 - NULL
    }
}

function fillInstallationPopup() {

    if (InstallationJson.length > 0) {
        var AddressType = InstallationJson[0].PostalAddressType;
        $("#JobInstallationDetails_AddressID").val(AddressType);
        if (AddressType == 1) {
            $("#JobInstallationDetails_UnitTypeID").val(InstallationJson[0].UnitType);
            $("#JobInstallationDetails_UnitNumber").val(InstallationJson[0].UnitNumber);
            $("#JobInstallationDetails_StreetNumber").val(InstallationJson[0].StreetNumber);
            $("#JobInstallationDetails_StreetName").val(InstallationJson[0].StreetName);
            $("#JobInstallationDetails_StreetTypeID").val(InstallationJson[0].StreetType);
            $("#JobInstallationDetails_PostalAddressID").val('');
            $("#JobInstallationDetails_PostalDeliveryNumber").val('');
        }
        else {
            $("#JobInstallationDetails_PostalAddressID").val(InstallationJson[0].PostalAddressID);
            $("#JobInstallationDetails_PostalDeliveryNumber").val(InstallationJson[0].PostalDeliveryNumber);
            $("#JobInstallationDetails_UnitTypeID").val("");
            $("#JobInstallationDetails_UnitNumber").val("");
            $("#JobInstallationDetails_StreetNumber").val("");
            $("#JobInstallationDetails_StreetName").val("");
            $("#JobInstallationDetails_StreetTypeID").val("");
        }

        $("#JobInstallationDetails_Town").val(InstallationJson[0].Town);
        $("#JobInstallationDetails_State").val(InstallationJson[0].State);
        $("#JobInstallationDetails_PostCode").val(InstallationJson[0].PostCode);        
    }

    else {
        $("#JobInstallationDetails_AddressID").val(1);
        $("#JobInstallationDetails_UnitTypeID").val("");
        $("#JobInstallationDetails_UnitNumber").val("");
        $("#JobInstallationDetails_StreetNumber").val("");
        $("#JobInstallationDetails_StreetName").val("");
        $("#JobInstallationDetails_StreetTypeID").val("");
        $("#JobInstallationDetails_Town").val('');
        $("#JobInstallationDetails_State").val('');
        $("#JobInstallationDetails_PostCode").val('');
        $("#JobInstallationDetails_PostalAddressID").val('');
        $("#JobInstallationDetails_PostalDeliveryNumber").val('');
        $("#JobInstallationDetails_IsSameAsOwnerAddress").prop('checked', false);
        $('.InstallationDPA').show();
        $('.InstallationPDA').hide();
    }

}


function fillOwnerPopup() {

    if (OwnerAddressJson.length > 0) {
        var AddressType = OwnerAddressJson[0].PostalAddressType;
        $("#JobOwnerDetails_AddressID").val(AddressType);
        if (AddressType == 1) {
            $("#JobOwnerDetails_UnitTypeID").val(OwnerAddressJson[0].UnitType);
            $("#JobOwnerDetails_UnitNumber").val(OwnerAddressJson[0].UnitNumber);
            $("#JobOwnerDetails_StreetNumber").val(OwnerAddressJson[0].StreetNumber);
            $("#JobOwnerDetails_StreetName").val(OwnerAddressJson[0].StreetName);
            $("#JobOwnerDetails_StreetTypeID").val(OwnerAddressJson[0].StreetType);
            $("#JobOwnerDetails_PostalAddressID").val('');
            $("#JobOwnerDetails_PostalDeliveryNumber").val('');
        }
        else {
            $("#JobOwnerDetails_PostalAddressID").val(OwnerAddressJson[0].PostalAddressID);
            $("#JobOwnerDetails_PostalDeliveryNumber").val(OwnerAddressJson[0].PostalDeliveryNumber);
            $("#JobOwnerDetails_UnitTypeID").val("");
            $("#JobOwnerDetails_UnitNumber").val("");
            $("#JobOwnerDetails_StreetNumber").val("");
            $("#JobOwnerDetails_StreetName").val("");
            $("#JobOwnerDetails_StreetTypeID").val("");
        }
        $('#JobOwnerDetails_CompanyName').val(OwnerAddressJson[0].CompanyName);
        $('#JobOwnerDetails_FirstName').val(OwnerAddressJson[0].FirstName);
        $('#JobOwnerDetails_LastName').val(OwnerAddressJson[0].LastName);
        $('#JobOwnerDetails_Email').val(OwnerAddressJson[0].Email);
        $('#JobOwnerDetails_Mobile').val(OwnerAddressJson[0].Mobile);
        $('#JobOwnerDetails_Phone').val(OwnerAddressJson[0].Phone);

        $("#JobOwnerDetails_Town").val(OwnerAddressJson[0].Town);
        $("#JobOwnerDetails_State").val(OwnerAddressJson[0].State);
        $("#JobOwnerDetails_PostCode").val(OwnerAddressJson[0].PostCode);
    }

    else {
        $("#JobOwnerDetails_AddressID").val(1);
        $("#JobOwnerDetails_UnitTypeID").val("");
        $("#JobOwnerDetails_UnitNumber").val("");
        $("#JobOwnerDetails_StreetNumber").val("");
        $("#JobOwnerDetails_StreetName").val("");
        $("#JobOwnerDetails_StreetTypeID").val("");
        $("#JobOwnerDetails_Town").val('');
        $("#JobOwnerDetails_State").val('');
        $("#JobOwnerDetails_PostCode").val('');
        $("#JobOwnerDetails_PostalAddressID").val('');
        $("#JobOwnerDetails_PostalDeliveryNumber").val('');

        $('#JobOwnerDetails_CompanyName').val('');
        $('#JobOwnerDetails_FirstName').val('');
        $('#JobOwnerDetails_LastName').val('');
        $('#JobOwnerDetails_Email').val('');
        $('#JobOwnerDetails_Mobile').val('');
        $('#JobOwnerDetails_Phone').val('');
        $('.OwnerDPA').show();
        $('.OwnerPDA').hide();
    }

}



function fillInstallerPopup() {
    if (InstallerJson.length > 0) {
        var AddressType = InstallerJson[0].PostalAddressType;
        $("#JobInstallerDetails_AddressID").val(AddressType);
        if (AddressType == 1) {
            $("#JobInstallerDetails_UnitTypeID").val(InstallerJson[0].UnitType);
            $("#JobInstallerDetails_UnitNumber").val(InstallerJson[0].UnitNumber);
            $("#JobInstallerDetails_StreetNumber").val(InstallerJson[0].StreetNumber);
            $("#JobInstallerDetails_StreetName").val(InstallerJson[0].StreetName);
            $("#JobInstallerDetails_StreetTypeID").val(InstallerJson[0].StreetType);
            $("#JobInstallerDetails_PostalAddressID").val('');
            $("#JobInstallerDetails_PostalDeliveryNumber").val('');
        }
        else {
            $("#JobInstallerDetails_PostalAddressID").val(InstallerJson[0].PostalAddressID);
            $("#JobInstallerDetails_PostalDeliveryNumber").val(InstallerJson[0].PostalDeliveryNumber);
            $("#JobInstallerDetails_UnitTypeID").val("");
            $("#JobInstallerDetails_UnitNumber").val("");
            $("#JobInstallerDetails_StreetNumber").val("");
            $("#JobInstallerDetails_StreetName").val("");
            $("#JobInstallerDetails_StreetTypeID").val("");
        }

        $('#JobInstallerDetails_FirstName').val(InstallerJson[0].FirstName);
        $('#JobInstallerDetails_Surname').val(InstallerJson[0].Surname);
        $('#JobInstallerDetails_Email').val(InstallerJson[0].Email);
        $('#JobInstallerDetails_Mobile').val(InstallerJson[0].Mobile);
        $('#JobInstallerDetails_Phone').val(InstallerJson[0].Phone);

        $("#JobInstallerDetails_Town").val(InstallerJson[0].Town);
        $("#JobInstallerDetails_State").val(InstallerJson[0].State);
        $("#JobInstallerDetails_PostCode").val(InstallerJson[0].PostCode);

        $("#JobInstallerDetails_ElectricianID").val(InstallerJson[0].ElectricianID);
        $("#JobInstallerDetails_SolarCompanyId").val(InstallerJson[0].SolarCompanyId);
        $("#JobInstallerDetails_SESignature").val(InstallerJson[0].SESignature);
    }

    else {
        $("#JobInstallerDetails_AddressID").val(1);
        $("#JobInstallerDetails_UnitTypeID").val("");
        $("#JobInstallerDetails_UnitNumber").val("");
        $("#JobInstallerDetails_StreetNumber").val("");
        $("#JobInstallerDetails_StreetName").val("");
        $("#JobInstallerDetails_StreetTypeID").val("");
        $("#JobInstallerDetails_Town").val('');
        $("#JobInstallerDetails_State").val('');
        $("#JobInstallerDetails_PostCode").val('');
        $("#JobInstallerDetails_PostalAddressID").val('');
        $("#JobInstallerDetails_PostalDeliveryNumber").val('');


        $('#JobInstallerDetails_FirstName').val('');
        $('#JobInstallerDetails_Surname').val('');
        $('#JobInstallerDetails_Email').val('');
        $('#JobInstallerDetails_Mobile').val('');
        $('#JobInstallerDetails_Phone').val('');
        $('.InstallerDPA').show();
        $('.InstallerPDA').hide();
    }

}

function EnableDropDownbyUsertype() {

    if (JOBType == '1') {
        if (USERType == 8 && IsUnderSSC == "True") {
            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobOwner").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });
            $("#JobInstallationAddress").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });
            $("#frmCreateJob").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });
            $("#BasicDetails_strInstallationDate").removeAttr("disabled");
        }
        if (USERType == 7 || USERType == 9) {
            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobElectricians").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });

            $("#JobOwner").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });

            $("#JobInstallationAddress").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });

            $("#frmCreateJob").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });

        }
        if (USERType == 2 || USERType == 5) {
            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobElectricians").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });

            $("#JobOwner").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });

            $("#JobInstallationAddress").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });

            $("#frmCreateJob").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });
            $("#BasicDetails_strInstallationDate").removeAttr("disabled");
        }
    }

    if (JOBType == '2') {
        if (USERType == 7 || USERType == 9 || (USERType == 8 && IsUnderSSC == "True")) {
            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobElectricians").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });

            $("#JobOwner").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });

            $("#JobInstallationAddress").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });

            $("#frmCreateJob").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });
            $("#BasicDetails_strInstallationDate").removeAttr("disabled");
        }

        if (USERType == 2 || USERType == 5) {
            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobElectricians").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });

            $("#JobOwner").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });

            $("#JobInstallationAddress").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });
            $("#Installer").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });
            $("#frmCreateJob").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });
            $("#BasicDetails_strInstallationDate").removeAttr("disabled");
        }
    }
}

function DisableDropDownbyUsertype() {
    if (BasicDetails_JobID > 0) {
        $("#BasicDetails_JobType").attr('disabled', 'disabled');
    }
    if (JOBType == '1') {

        if (USERType == 8 && IsUnderSSC == "True") {
            $("#JobOwner").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });
            $("#JobInstallationAddress").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });
            $("#frmCreateJob").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });
            $("#BasicDetails_strInstallationDate").attr('disabled', 'disabled');
        }
        if (USERType == 7 || USERType == 9) {
            $("#JobElectricians").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });

            $("#JobOwner").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });

            $("#JobInstallationAddress").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });

            $("#frmCreateJob").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });

            $('#BasicDetails_JobStage').removeAttr("disabled");

        }
        if (USERType == 2 || USERType == 5) {
            $("#JobElectricians").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });

            $("#JobOwner").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });

            $("#JobInstallationAddress").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });

            $("#frmCreateJob").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });
            $("#BasicDetails_strInstallationDate").attr('disabled', 'disabled');
        }
    }

    if (JOBType == '2') {
        if (USERType == 7 || USERType == 9 || (USERType == 8 && IsUnderSSC == "True")) {
            $("#JobElectricians").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });

            $("#JobOwner").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });

            $("#JobInstallationAddress").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });

            $("#frmCreateJob").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });
            $("#BasicDetails_strInstallationDate").attr('disabled', 'disabled');
            $('#BasicDetails_JobStage').removeAttr("disabled");
        }


        if (USERType == 2 || USERType == 5) {
            $("#JobElectricians").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });

            $("#JobOwner").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });

            $("#JobInstallationAddress").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });
            $("#Installer").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });
            $("#frmCreateJob").find('Select').each(function () {

                $(this).attr('disabled', 'disabled');
            });
            $("#BasicDetails_strInstallationDate").attr('disabled', 'disabled');
        }
    }
}

function CalculateStc1() {

    $("#JobSystemDetails_CalculatedSTC").val('');
    $("#JobSystemDetails_CalculatedSTCForSWH").val();
    var JobType = $("#BasicDetails_JobType").val();
    var systemsize = $("#JobSystemDetails_SystemSize").val();

    var installationDate = $("#BasicDetails_strInstallationDate").val();
    var expectedInstallDate = "";
    if (installationDate != null && installationDate != undefined && installationDate != '') {
        var installDate = ConvertDateToTick(installationDate, GetDateFormat);
        expectedInstallDate = moment(installDate).format("YYYY-MM-DD");
    }

    var deemingPeriod = $("#JobSTCDetails_DeemingPeriod").val();
    var postcode = InstallationPostcodeFromjson;
    var systemBrand = $("#JobSystemDetails_SystemBrand").val();
    var systemModel = $("#JobSystemDetails_SystemModel").val();


    if (JobType == 1) {
        if (expectedInstallDate != null && expectedInstallDate != "" && deemingPeriod != null && deemingPeriod != "" && postcode != null && postcode != "" && systemsize != null && systemsize != "") {
            $.ajax(
                {
                    url: urlCalculateSTC,
                    data: { sguType: 'SolarDeemed', expectedInstallDate: expectedInstallDate, deemingPeriod: deemingPeriod, postcode: postcode, systemsize: systemsize },
                    contentType: 'application/json',
                    method: 'get',
                    success: function (data) {
                        var obj = JSON.parse(data);
                        if (obj.status == "Failed") {
                            $("#spanJobSystemDetails_CalculatedSTC").show().text(obj.fieldErrors[0].defaultMessage);
                            //alert(obj.fieldErrors[0].defaultMessage);

                        }
                        else {
                            var numberOfStcs = obj.result.numberOfStcs;
                            $("#JobSystemDetails_CalculatedSTC").val(numberOfStcs);
                        }

                    }
                });
        }

        else {
            $("#spanJobSystemDetails_CalculatedSTC").show().text("Installation Date, STC DeemingPeriod,Installation postcode and System size are required for it.").fadeOut(6000);

        }
    }

    else if (JobType == 2) {
        if (expectedInstallDate != null && expectedInstallDate != "" && expectedInstallDate != undefined && postcode != null && postcode != "" && postcode != undefined && systemBrand != null && systemBrand != "" && systemModel != null && systemModel != "") {
            $.ajax(
                {
                    url: urlCalculateSWHST,
                    data: { expectedInstallDate: expectedInstallDate, postcode: postcode, systemBrand: systemBrand, systemModel: systemModel },
                    contentType: 'application/json',
                    method: 'get',
                    success: function (data) {
                        var obj = JSON.parse(data);
                        if (obj.status == "Failed") {
                            $("#spanJobSystemDetails_CalculatedSTCForSWH").show().text(obj.errors[0]);
                            //alert(obj.fieldErrors[0].defaultMessage);

                        }
                        else {
                            var numberOfStcs = obj.result.numStc;
                            $("#JobSystemDetails_CalculatedSTCForSWH").val(numberOfStcs);
                        }

                    }
                });
        }

        else {
            $("#spanJobSystemDetails_CalculatedSTCForSWH").show().text('Installation Date,System brand,System Model,Owner postcode are required for it.');

        }

    }
    else {
        $("#spanJobSystemDetails_CalculatedSTC").show().text("Please select Job Type.").fadeOut(6000);
        $("#spanJobSystemDetails_CalculatedSTCForSWH").show().text("Please select Job Type.").fadeOut(6000);

    }

}

function CheckStatusAndInstallationDate() {
    var message;
    var installerId = $("#BasicDetails_InstallerID").val();
    var designerId = $("#BasicDetails_DesignerID").val();

    var installationDate = $("#BasicDetails_strInstallationDate").val();

    if (designerId == "" || designerId == undefined || designerId == null) {
        designerId = "0";
    }
    if (installerId == "" || installerId == undefined || installerId == null) {
        installerId = "0";
    }

    if ((installerId != 0 || designerId != 0) && $("#BasicDetails_JobType").val() == 1) {
        var IsReturn = true;
        $.ajax(
                   {
                       url: urlCheckStatusAndInstallationDate,
                       data: { InstallerId: installerId, DesignerId: designerId },
                       contentType: 'application/json',
                       method: 'get',
                       async: false,
                       success: function (data) {

                           var obj = JSON.parse(data);

                           for (i = 0; i < obj.length ; i++) {
                               var status = obj[i].InstallerStatus;
                               var expirydate = obj[i].InstallerExpiryDate;
                               var SuspensionStartDate = obj[i].SuspensionStartDate;
                               var SuspensionEndDate = obj[i].SuspensionEndDate;


                               if (installationDate != null) {
                                   var tickEndDate = ConvertDateToTick(installationDate, GetDateFormat);
                                   installationDate = moment(tickEndDate).format("YYYY-MM-DD");
                               }
                               if (expirydate != null) {
                                   expirydate = moment(expirydate).format("YYYY-MM-DD");
                               }
                               if (SuspensionStartDate != null) {
                                   SuspensionStartDate = moment(SuspensionStartDate).format("YYYY-MM-DD");
                               }
                               if (SuspensionStartDate != null) {
                                   SuspensionStartDate = moment(SuspensionStartDate).format("YYYY-MM-DD");
                               }

                               if (status == "Deferred") {
                                   message = "The account is Deferred.";
                                   showErrorMessagesForStatus(message);
                                   IsReturn = false;
                                   break;
                               }
                               if (status == "Cancelled") {
                                   message = "The account is Cancelled.";
                                   showErrorMessagesForStatus(message);
                                   IsReturn = false;
                                   break;
                               }
                               if (status == "Suspended") {
                                   if (installationDate != null && installationDate != "" && installationDate != undefined) {
                                       if (SuspensionStartDate < installationDate < SuspensionEndDate) {
                                           message = "The account is suspended.";
                                           showErrorMessagesForStatus(message);
                                           IsReturn = false;
                                           break;
                                       }
                                       else {
                                           if (expirydate < installationDate) {
                                               IsReturn = true;
                                           }
                                           else {
                                               message = "The account is expired.";
                                               showErrorMessagesForStatus(message);
                                               IsReturn = false;
                                               break;

                                           }
                                       }
                                   }
                                   else {
                                       message = "The account is suspended.";
                                       showErrorMessagesForStatus(message);
                                       IsReturn = false;
                                       break;
                                   }
                               }
                               if (status == "Expired") {
                                   if (installationDate != null && installationDate != "" && installationDate != undefined) {
                                       if (expirydate < installationDate) {
                                           IsReturn = true;
                                       }
                                       else {
                                           message = "The S.E account is expired";
                                           showErrorMessagesForStatus(message);
                                           IsReturn = false;
                                           break;
                                       }
                                   }
                                   else {
                                       message = "The S.E account is expired.";
                                       showErrorMessagesForStatus(message);
                                       IsReturn = false;
                                       break;
                                   }
                               }
                               if (status == "Current") {
                                   if (installationDate != null && installationDate != "" && installationDate != undefined) {
                                       if (SuspensionStartDate < installationDate < SuspensionEndDate) {
                                           message = "The account is suspended.";
                                           showErrorMessagesForStatus(message);
                                           IsReturn = false;
                                           break;
                                       }
                                       else {
                                           if (expirydate < installationDate) {
                                               IsReturn = true;
                                           }
                                           else {
                                               message = "The account is expired.";
                                               showErrorMessagesForStatus(message);
                                               IsReturn = false;
                                               break;

                                           }
                                       }
                                   }
                                   else {
                                       IsReturn = true;
                                   }
                               }
                           }
                       },
                       error: function () {
                           IsReturn = true;
                       }

                   });
        return IsReturn;
    }
    else {
        return true;
    }
}


function FillDropDownInstaller(id, url, value, hasSelect, callback, defaultText) {
    $.ajax(
        {
            url: url,
            contentType: 'application/json',
            dataType: 'json',
            method: 'get',
            cache: false,
            success: function (success) {
                var options = '';

                if (hasSelect == true) {
                    if (defaultText == undefined || defaultText == null)
                        defaultText = 'Select';

                    options = "<option value=''>" + defaultText + "</option>";
                }

                $.each(success, function (i, val) {
                    if (val.Text.indexOf("(Current)") >= 0) {
                        options += '<option value = "' + val.Value + '" style="color:green" >' + val.Text + '</option>'
                    }
                    else if (val.Text.indexOf("(") >= 0 && val.Text.indexOf("(Current)") < 0) {
                        options += '<option value = "' + val.Value + '"  style="color:red">' + val.Text + '</option>'
                    }
                    else {
                        options += '<option value = "' + val.Value + '"  >' + val.Text + '</option>'
                    }
                });

                $("#" + id).html(options);

                if (value && value != '' && value != 0) {
                    $("#" + id).val(value);
                }

                if ($('#' + id).attr('selval') && $('#' + id).attr('selval') > 0) {
                    $("#" + id).val($('#' + id).attr('selval'));
                }

                if ($("#" + id).selectpicker != undefined) {
                    $("#" + id).selectpicker('refresh');
                }

                if (callback != undefined) {
                    callback();
                    //setDropDownWidth(id);
                }
            }
        });
}

function showSuccessMessages(message) {
    $(".alert").hide();
    $("#successMsgRegion").html(closeButton + message);
    $("#successMsgRegion").show();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');
}
function showErrorMessagesForStatus(message) {
    $(".alert").hide();
    $("#errorMsgRegion").html(closeButton + message);
    $("#errorMsgRegion").show();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');
}
function addressElectricianValidationRules() {
    $("#JobElectricians_UnitTypeID").rules("add", {
        required: false,
    });
    $("#JobElectricians_UnitNumber").rules("add", {
        required: false,
    });
    if ($("#JobElectricians_UnitTypeID").val() == "" && $("#JobElectricians_UnitNumber").val() == "") {
        $('#lblElectriciansUnitNumber').removeClass("required");
        $('#lblElectriciansUnitTypeID').removeClass("required");
        $("#JobElectricians_UnitNumber").rules("add", {
            required: false,
        });
        $("#JobElectricians_UnitTypeID").rules("add", {
            required: false,
        });
        $('#lblElectriciansStreetNumber').addClass("required");
        $("#JobElectricians_StreetNumber").rules("add", {
            required: true,
            messages: {
                required: "Street Number is required."
            }
        });
    }

    if ($("#JobElectricians_UnitTypeID").val() > 0 && $("#JobElectricians_UnitNumber").val() != "") {
        $('#lblElectriciansStreetNumber').removeClass("required");
        $("#JobElectricians_StreetNumber").rules("add", {
            required: false,
        });
        $('#lblElectriciansUnitNumber').removeClass("required");
        $('#lblElectriciansUnitTypeID').removeClass("required");
        $("#JobElectricians_UnitNumber").rules("add", {
            required: false,
        });
        $("#JobElectricians_UnitTypeID").rules("add", {
            required: false,
        });
    }
    if ($("#JobElectricians_UnitTypeID").val() > 0 && $("#JobElectricians_UnitNumber").val() == "") {
        $("#JobElectricians_UnitNumber").rules("add", {
            required: true,
            messages: {
                required: "Unit Number is required."
            }
        });
        $('#lblElectriciansUnitNumber').addClass("required");
        $('#lblElectriciansStreetNumber').removeClass("required");
        $("#JobElectricians_StreetNumber").rules("add", {
            required: false,
        });
    }
    if ($("#JobElectricians_UnitTypeID").val() == "" && $("#JobElectricians_UnitNumber").val() != "") {
        $('#lblElectriciansUnitNumber').addClass("required");
        $('#lblElectriciansUnitTypeID').removeClass("required");
        $("#JobElectricians_UnitNumber").rules("add", {
            required: false,
        });
        $("#JobElectricians_UnitTypeID").rules("add", {
            required: false,
        });
        $('#lblElectriciansStreetNumber').addClass("required");
        $("#JobElectricians_StreetNumber").rules("add", {
            required: true,
            messages: {
                required: "Street Number is required."
            }
        });
    }
}

function addressOwnerValidationRules() {


    $("#JobOwnerDetails_UnitTypeID").rules("add", {
        required: false,
    });
    $("#JobOwnerDetails_UnitNumber").rules("add", {
        required: false,
    });
    if ($("#JobOwnerDetails_UnitTypeID").val() == "" && $("#JobOwnerDetails_UnitNumber").val() == "") {
        $('#lblOwnerUnitNumber').removeClass("required");
        $('#lblOwnerUnitTypeID').removeClass("required");
        $("#JobOwnerDetails_UnitNumber").rules("add", {
            required: false,
        });
        $("#JobOwnerDetails_UnitTypeID").rules("add", {
            required: false,
        });
        $('#lblOwnerStreetNumber').addClass("required");
        $("#JobOwnerDetails_StreetNumber").rules("add", {
            required: true,
            messages: {
                required: "Street Number is required."
            }
        });
    }

    if ($("#JobOwnerDetails_UnitTypeID").val() > 0 && $("#JobOwnerDetails_UnitNumber").val() != "") {
        $('#lblOwnerStreetNumber').removeClass("required");
        $("#JobOwnerDetails_StreetNumber").rules("add", {
            required: false,
        });
        $('#lblOwnerUnitNumber').removeClass("required");
        $('#lblOwnerUnitTypeID').removeClass("required");
        $("#JobOwnerDetails_UnitNumber").rules("add", {
            required: false,
        });
        $("#JobOwnerDetails_UnitTypeID").rules("add", {
            required: false,
        });
    }
    if ($("#JobOwnerDetails_UnitTypeID").val() > 0 && $("#JobOwnerDetails_UnitNumber").val() == "") {
        $("#JobOwnerDetails_UnitNumber").rules("add", {
            required: true,
            messages: {
                required: "Unit Number is required."
            }
        });
        $('#lblOwnerUnitNumber').addClass("required");
        $('#lblOwnerStreetNumber').removeClass("required");
        $("#JobOwnerDetails_StreetNumber").rules("add", {
            required: false,
        });
    }
    if ($("#JobOwnerDetails_UnitTypeID").val() == "" && $("#JobOwnerDetails_UnitNumber").val() != "") {
        $('#lblOwnerUnitNumber').addClass("required");
        $('#lblOwnerUnitTypeID').removeClass("required");
        $("#JobOwnerDetails_UnitNumber").rules("add", {
            required: false,
        });
        $("#JobOwnerDetails_UnitTypeID").rules("add", {
            required: false,
        });
        $('#lblOwnerStreetNumber').addClass("required");
        $("#JobOwnerDetails_StreetNumber").rules("add", {
            required: true,
            messages: {
                required: "Street Number is required."
            }
        });
    }
}

function addressInstallationValidationRules() {


    $("#JobInstallationDetails_UnitTypeID").rules("add", {
        required: false,
    });
    $("#JobInstallationDetails_UnitNumber").rules("add", {
        required: false,
    });
    if ($("#JobInstallationDetails_UnitTypeID").val() == "" && $("#JobInstallationDetails_UnitNumber").val() == "") {
        $('#lblInstallationUnitNumber').removeClass("required");
        $('#lblInstallationUnitTypeID').removeClass("required");
        $("#JobInstallationDetails_UnitNumber").rules("add", {
            required: false,
        });
        $("#JobInstallationDetails_UnitTypeID").rules("add", {
            required: false,
        });
        //$('#lblInstallationStreetNumber').addClass("required");
        //$("#JobInstallationDetails_StreetNumber").rules("add", {
        //    required: true,
        //    messages: {
        //        required: "Street Number is required."
        //    }
        //});
    }

    if ($("#JobInstallationDetails_UnitTypeID").val() > 0 && $("#JobInstallationDetails_UnitNumber").val() != "") {
        //$('#lblInstallationStreetNumber').removeClass("required");
        //$("#JobInstallationDetails_StreetNumber").rules("add", {
        //    required: false,
        //});
        $('#lblInstallationUnitNumber').removeClass("required");
        $('#lblInstallationUnitTypeID').removeClass("required");
        $("#JobInstallationDetails_UnitNumber").rules("add", {
            required: false,
        });
        $("#JobInstallationDetails_UnitTypeID").rules("add", {
            required: false,
        });
    }
    if ($("#JobInstallationDetails_UnitTypeID").val() > 0 && $("#JobInstallationDetails_UnitNumber").val() == "") {
        $("#JobInstallationDetails_UnitNumber").rules("add", {
            required: true,
            messages: {
                required: "Unit Number is required."
            }
        });
        $('#lblInstallationUnitNumber').addClass("required");
        //$('#lblInstallationStreetNumber').removeClass("required");
        //$("#JobInstallationDetails_StreetNumber").rules("add", {
        //    required: false,
        //});
    }
    if ($("#JobInstallationDetails_UnitTypeID").val() == "" && $("#JobInstallationDetails_UnitNumber").val() != "") {
        $('#lblInstallationUnitNumber').addClass("required");
        $('#lblInstallationUnitTypeID').removeClass("required");
        $("#JobInstallationDetails_UnitNumber").rules("add", {
            required: false,
        });
        $("#JobInstallationDetails_UnitTypeID").rules("add", {
            required: false,
        });
        //$('#lblInstallationStreetNumber').addClass("required");
        //$("#JobInstallationDetails_StreetNumber").rules("add", {
        //    required: true,
        //    messages: {
        //        required: "Street Number is required."
        //    }
        //});
    }
}


function addressInstallerValidationRules() {


    $("#JobInstallerDetails_UnitTypeID").rules("add", {
        required: false,
    });
    $("#JobInstallerDetails_UnitNumber").rules("add", {
        required: false,
    });
    if ($("#JobInstallerDetails_UnitTypeID").val() == "" && $("#JobInstallerDetails_UnitNumber").val() == "") {
        $('#lblInstallerUnitNumber').removeClass("required");
        $('#lblInstallerUnitTypeID').removeClass("required");
        $("#JobInstallerDetails_UnitNumber").rules("add", {
            required: false,
        });
        $("#JobInstallerDetails_UnitTypeID").rules("add", {
            required: false,
        });
        $('#lblInstallerStreetNumber').addClass("required");
        $("#JobInstallerDetails_StreetNumber").rules("add", {
            required: true,
            messages: {
                required: "Street Number is required."
            }
        });
    }

    if ($("#JobInstallerDetails_UnitTypeID").val() > 0 && $("#JobInstallerDetails_UnitNumber").val() != "") {
        $('#lblInstallerStreetNumber').removeClass("required");
        $("#JobInstallerDetails_StreetNumber").rules("add", {
            required: false,
        });
        $('#lblInstallerUnitNumber').removeClass("required");
        $('#lblInstallerUnitTypeID').removeClass("required");
        $("#JobInstallerDetails_UnitNumber").rules("add", {
            required: false,
        });
        $("#JobInstallerDetails_UnitTypeID").rules("add", {
            required: false,
        });
    }
    if ($("#JobInstallerDetails_UnitTypeID").val() > 0 && $("#JobInstallerDetails_UnitNumber").val() == "") {
        $("#JobInstallerDetails_UnitNumber").rules("add", {
            required: true,
            messages: {
                required: "Unit Number is required."
            }
        });
        $('#lblInstallerUnitNumber').addClass("required");
        $('#lblInstallerStreetNumber').removeClass("required");
        $("#JobInstallerDetails_StreetNumber").rules("add", {
            required: false,
        });
    }
    if ($("#JobInstallerDetails_UnitTypeID").val() == "" && $("#JobInstallerDetails_UnitNumber").val() != "") {
        $('#lblInstallerUnitNumber').addClass("required");
        $('#lblInstallerUnitTypeID').removeClass("required");
        $("#JobInstallerDetails_UnitNumber").rules("add", {
            required: false,
        });
        $("#JobInstallerDetails_UnitTypeID").rules("add", {
            required: false,
        });
        $('#lblInstallerStreetNumber').addClass("required");
        $("#JobInstallerDetails_StreetNumber").rules("add", {
            required: true,
            messages: {
                required: "Street Number is required."
            }
        });
    }
}

function checkBusinessRules(saveType, isFromSTC) {
    if ($("#SSCID").val() != "" && $("#SSCID").val() > 0) {
        $("#BasicDetails_SSCID").val($("#SSCID").val());
    }
    if ($("#ScoID").val() != "" && $("#ScoID").val() > 0) {
        $("#BasicDetails_ScoID").val($("#ScoID").val());
    }
    //dateaa =$("#BasicDetails_strInstallationDate").val();
    //else if (($('#spanSTCStatus').text().trim() == 'Not Yet Submitted' || $('#spanSTCStatus').text().trim() == 'Ready to Trade' || $('#spanSTCStatus').text().trim() == 'Compliance Issues' || $('#spanSTCStatus').text().trim() == 'Failure due to REC Audit') && isFromSTC) {
    //    isCheckRules = true;
    //}

    //Date Convert.


    EnableDropDownbyUsertype();
    $("#BasicDetails_JobType").removeAttr("disabled");
    var panelData = '';
    var xmlPanel = '';
    var inverterData = '';
    var xmlInverter = '';
    var batteryManufacturerData = [];
    batteryManufacturerData = JSON.parse(JSON.stringify(batteryXml));

    if (PanelXml.length > 0) {
        var jsonp = JSON.stringify(PanelXml);
        var sName = '';
        var obj = $.parseJSON(jsonp);
        $.each(obj, function () {
            sName += '<panel><Brand>' + this['Brand'] + '</Brand>' + '<Model>' + this['Model'] + '</Model>' +
                '<NoOfPanel>' + this['Count'] + '</NoOfPanel></panel>';
        });
        xmlPanel = '<Panels>' + sName + '</Panels>'
    }
    if (InverterXml.length > 0) {

        var jsonp = JSON.stringify(InverterXml);
        var sName = '';
        var obj = $.parseJSON(jsonp);
        $.each(obj, function () {
            sName += '<inverter><Brand>' + htmlEncode(this['Brand']) + '</Brand>' + '<Model>' + htmlEncode(this['Model']) + '</Model>' +
                '<Series>' + htmlEncode(this['Series']) + '</Series></inverter>';
        });
        xmlInverter = '<Inverters>' + sName + '</Inverters>';
    }

    $("#panelXml").val(xmlPanel);
    $("#inverterXml").val(xmlInverter);
    var data = JSON.stringify($('form').serializeToJson());
    var objData = JSON.parse(data);

    objData.JobSTCDetails.batterySystemPartOfAnAggregatedControl = $("#JobSTCDetails_batterySystemPartOfAnAggregatedControl").val();
    objData.JobSTCDetails.changedSettingOfBatteryStorageSystem = $("#JobSTCDetails_changedSettingOfBatteryStorageSystem").val();

    if (batteryManufacturerData.length > 0) {

        for (var i = 0; i < batteryManufacturerData.length; i++) {
            delete batteryManufacturerData[i]["JobBatteryManufacturerId"];
        }
        objData.lstJobBatteryManufacturer = batteryManufacturerData;
    }

    data = JSON.stringify(objData);

    var data1 = JSON.stringify($('#frmCreateJob').serializeToJson());

    $.ajax({
        url: urlCheckBusinessRules,
        type: "POST",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8', // Not to set any content header
        processData: false, // Not to process data
        data: data,
        cache: false,
        async: true,
        success: function (result) {                    
            if (result.IsSuccess) {
                if (result.STCStatusName != null && result.STCStatusName != undefined && result.STCStatusName != '') {
                    $('#spanSTCStatus').text(result.STCStatusName.trim());
                }

                if (result.STCDescription != null && result.STCDescription != undefined && result.STCDescription != '') {
                    $('#STCDescription').html(result.STCDescription.trim());
                }

                if (result.STCStatusId != null && result.STCStatusId != undefined && result.STCStatusId != '') {
                    $('#STCStatusId').val(result.STCStatusId.trim());
                }

                if (result.ValidationSummary != null && result.ValidationSummary != undefined && result.ValidationSummary != '') {
                    if (isFromSTC) {
                        if (ProjectSession_UserTypeId != 1) {
                            $(".STCmodelBodyMessage").html('');
                            $(".STCmodelBodyMessage").append(result.ValidationSummary);
                            $("#STCwarning").modal();
                            if (result.IsEMailNotification.toString().toLowerCase() == 'true') {
                                //Email configuration not required

                                $.ajax({
                                    url: urlSendMailForDuplicateAddress,
                                    method: 'GET',
                                    data: { jobid: $('#BasicDetails_JobID').val(), mailList: result.EMailList.trim().toString().toLowerCase() },
                                    cache: false,
                                    async: true,
                                    success: function (Data) {

                                        //alert(Data);
                                    }
                                });
                                //}
                                //else
                                //{
                                //    $(".alert").hide();
                                //    $("#errorMsgRegion").removeClass("alert-success");
                                //    $("#errorMsgRegion").addClass("alert-danger");
                                //    $("#errorMsgRegion").html(closeButton + 'Can not send mail for Duplicate installation address. Please configure your email account');
                                //    $("#errorMsgRegion").show();
                                //}
                            }
                        }
                        else {
                            LoadStc();
                        }

                    } else {
                        $(".modelBodyMessage").html('');
                        $(".modelBodyMessage").append(result.ValidationSummary);
                        $("#warning").modal();
                    }
                }
                else {
                    if (isFromSTC) {
                        LoadStc();
                    } else {
                        SaveJob(saveType);
                    }
                }
            }
            else if (result.IsError) {

                $("#errorMsgRegion").html(closeButton + result.ValidationSummary);
                //$("#errorMsgRegion").html(closeButton + "The field cannot contain symbols like < > ^ [ ].");
                $("#errorMsgRegion").show();
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');
                DisableDropDownbyUsertype();
            }
        }
    });
}
function jobChange() {
    if ($('#BasicDetails_JobType').val() == "2") {
        $("#TypeOfConnection,#JobSTCDetails_TypeOfConnection,#SystemMountingType,#JobSTCDetails_SystemMountingType").hide();
        $("#DeemingPeriod,#JobSTCDetails_DeemingPeriod,#InstallerID,#BasicDetails_InstallerID,#DesignerID,#BasicDetails_DesignerID").hide();
        //$("#DeemingPeriod,#JobSTCDetails_DeemingPeriod,#InstallerID,#DesignerID,#BasicDetails_DesignerID").hide();
        $("#NMI,#JobInstallationDetails_NMI,#InstallingNewPanel,#InstallingNewPanel,#InstallingNewPanel,#JobInstallationDetails_InstallingNewPanel,#MeterNumber,#JobInstallationDetails_MeterNumber,#PhaseProperty,#JobInstallationDetails_PhaseProperty,#ElectricityProviderID,#JobInstallationDetails_ElectricityProviderID,#ExistingSystem1,#JobInstallationDetails_ExistingSystem,#ExistingSystem").hide();
        $("#installContent").show();
        $("#serialNumberLable").html("Tank serial number(s):");
        $("#MultipleSGUAddress").html("Is there more than one SWH/ASHP at this address:");
        $("#CertificateCreated").html("Creating certificates for previously failed SWH:");
        $(".SWSystemDetails").show();
        $(".PVDSystemDetail").hide();
        $("#divVolumetricCapacity").show();
        $("#divSecondhandWaterHeater").show();
        $("#jobSSCID").hide();
        $("#JobSystemDetails_NoOfPanel").removeAttr("readonly");
        $("#electricianDetail").hide();    
        FillDropDown('JobInstallerDetails_ElectricianID', urlGetElectrician + $("#BasicDetails_JobID").val() + '&solarCompanyId=' + $("#BasicDetails_SolarCompanyId").val() + '&JobType=' + $("#BasicDetails_JobType").val(), JobInstallerDetails_ElectricianID, true, null);        

    } else if ($('#BasicDetails_JobType').val() == "1") {
        $("#TypeOfConnection,#JobSTCDetails_TypeOfConnection,#SystemMountingType,#JobSTCDetails_SystemMountingType").show();
        $("#DeemingPeriod,#JobSTCDetails_DeemingPeriod,#InstallerID,#BasicDetails_InstallerID,#DesignerID,#BasicDetails_DesignerID").show();
        $("#NMI,#JobInstallationDetails_NMI,#InstallingNewPanel,#InstallingNewPanel,#InstallingNewPanel,#JobInstallationDetails_InstallingNewPanel,#MeterNumber,#JobInstallationDetails_MeterNumber,#PhaseProperty,#JobInstallationDetails_PhaseProperty,#ElectricityProviderID,#JobInstallationDetails_ElectricityProviderID,#ExistingSystem1,#JobInstallationDetails_ExistingSystem").show();
        $("#installContent").hide();
        $("#serialNumberLable").html("Equipment model serial number(s):");
        $("#MultipleSGUAddress").html("Is there more than one SGU at this address?:");
        $("#CertificateCreated").html("Are you creating certificates for a system that has previously been failed by the Clean Energy Regulator?:");
        $(".SWSystemDetails").hide();
        $(".PVDSystemDetail").show();
        $("#divVolumetricCapacity").hide();
        $("#divStatutoryDeclarations").hide();
        $("#divSecondhandWaterHeater").hide();
        $("#jobSSCID").show();
        $("#JobSystemDetails_NoOfPanel").attr("readonly");
        $("#electricianDetail").show();

        FillDropDown('JobElectricians_ElectricianID', urlGetElectrician + $("#BasicDetails_JobID").val() + '&solarCompanyId=' + $("#BasicDetails_SolarCompanyId").val(), JobElectricians_ElectricianID, true, null);

    }
    FillDropDown('JobPanelDetails_Brand', urlGetPanelBrand0 + $('#BasicDetails_JobType').val(), 0, true, null);
}

function GSTFileUpload() {
    var Guid = Model_Guid;
    //var jobId = $("#BasicDetails_JobID").val();
    var jobId = Guid;
    var url = urlUploadInvoice;
    $('#uploadGSTFile').fileupload({

        url: url,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;

            var UploadFailedFilesName = [];
            UploadFailedFilesName.length = 0;

            var UploadFailedFilesType = [];
            UploadFailedFilesType.length = 0;
            //formbot start
            for (var i = 0; i < data.result.length; i++) {

                if (data.result[i].Status == true) {

                    var Sign = document.getElementsByName("BasicDetails.GSTDocument");
                    var OldFileName = $("#BasicDetails_GSTDocument").attr('OldFileName');
                    if (Sign.length > 0) {
                        var signName = Sign[0].defaultValue;
                        if (signName != null && signName != "" && (OldFileName == '' || OldFileName == null || OldFileName == undefined)) {
                            DeleteGSTFile(signName, jobId);
                        }
                        $("#tblDocuments").find('tr').each(function () {
                            $(this).remove();
                        });
                    }

                    //$("[name='BasicDetails.GSTDocument']").each(function () {
                    //    $(this).remove();
                    //});

                    //var rowcount = $('#tblDocuments tr').length;
                    //var count = rowcount + 1;
                    var documentType = data.result[i].MimeType.split("/");
                    var mimeType = documentType[0];
                    //var documentId = "document" + count;
                    var content = "<tr style='margin-top:30px' id= " + data.result[i].FileName.replace("%", "$") + " >"
                    //content += '<td class="tdCount col-sm-2" >' + count + '.' + ' </td>';

                    //var originalFileName = data.result[i].FileName.substr(data.result[i].FileName.replace("%", "$").indexOf('_') + 1);

                    content += '<td class="col-sm-8" style="color:#494949;border-bottom:0px !important">' + data.result[i].FileName.replace("%", "$") + ' </td>';



                    if (mimeType == "image") {
                        content += '<td  class="col-sm-2" style="color:blue;border-bottom:0px !important"><img src=' + ProjectImagePath + 'images/view-icon.png style="cursor: pointer" id="' + data.result[i].FileName.replace("%", "$") + '"  class="' + data.result[i].FileName.replace("%", "$") + '" title="Preview" onclick="OpenGSTDocument(this)"></td>';
                    }
                    else {
                        content += '<td  class="col-sm-2" style="color:blue;border-bottom:0px !important"><img src=' + ProjectImagePath + 'images/view-icon.png style="cursor: pointer" id="' + data.result[i].FileName.replace("%", "$") + '" class="' + data.result[i].FileName.replace("%", "$") + '" title="Download" onclick="DownloadGSTDocument(this)"></td>';
                    }
                    content += '<td class="col-sm-2" style="color:blue;border-bottom:0px !important"><img src=' + ProjectImagePath + 'images/delete-icon.png style="cursor: pointer" id="' + data.result[i].FileName.replace("%", "$") + '" title="Delete" onclick="DeleteGSTDocument(this)"></td>';
                    content += "</tr>"

                    $('#tblDocuments').append(content);

                    $('#BasicDetails_GSTDocument').val(data.result[i].FileName.replace("%", "$"));


                    //$('<input type="hidden">').attr({
                    //    name: 'GSTDoc',
                    //    value: data.result[i].FileName.replace("%", "$"),
                    //    id: data.result[i].FileName.replace("%", "$"),
                    //}).appendTo('#frmCreateJob');

                }
                else if (data.result[i].Status == false && data.result[i].Message == "File Type Not Allowed.") {
                    UploadFailedFilesType.push(data.result[i].FileName);
                }
                else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                    UploadFailedFilesName.push(data.result[i].FileName.replace("%", "$"));
                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName.replace("%", "$"));

                }
            }
            if (UploadFailedFiles.length > 0) {

                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFiles.length + " " + "File has not been uploaded.");
                $("#errorMsgRegion").show();
            }
            if (UploadFailedFilesType.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFilesType.length + " " + "Uploaded file type is not supported.");
                $("#errorMsgRegion").show();
            }
            if (UploadFailedFilesName.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFilesName.length + " " + "Uploaded filename is too big.");
                $("#errorMsgRegion").show();
            }
            if (UploadFailedFilesName.length == 0 && UploadFailedFilesType.length == 0 && UploadFailedFiles.length == 0) {
                $(".alert").hide();
                $("#errorMsgRegion").hide();
                $("#successMsgRegion").html(closeButton + "File has been uploaded successfully.");
                $("#successMsgRegion").show();
            }
        },

        progressall: function (e, data) {

        },

        singleFileUploads: false,
        send: function (e, data) {
            if (data.files.length == 1) {
                for (var i = 0; i < data.files.length; i++) {

                    if (data.files[i].name.length > 500) {
                        $(".alert").hide();
                        $("#successMsgRegion").hide();
                        $("#errorMsgRegion").html(closeButton + "Please upload small filename.");
                        $("#errorMsgRegion").show();
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');

                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        ShowErrorMsgForFileName("Please upload file that not conatain <> ^ [] .")
                        return false;
                    }
                }
            }
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(MaxImageSize)) {

                        $(".alert").hide();
                        $("#successMsgRegion").hide();
                        $("#errorMsgRegion").html(closeButton + " " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB");
                        $("#errorMsgRegion").show();

                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        ShowErrorMsgForFileName("Please upload file that not conatain <> ^ [] .")
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(MaxImageSize)) {
                    $(".alert").hide();
                    $("#successMsgRegion").hide();
                    $("#errorMsgRegion").html(closeButton + "Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxsize + "MB");
                    $("#errorMsgRegion").show();

                    return false;
                }
            }
            $(".alert").hide();
            $("#errorMsgRegion").html("");
            $("#errorMsgRegion").hide();
            $('<input type="hidden">').attr({
                name: 'Guid',
                value: USERID,
                id: USERID,
            }).appendTo('form');
            return true;
        },
        //formData: { userId: USERID },
        formData: { guid: Guid, jobId: jobId, isGSTDocument: 1 },
        change: function (e, data) {
            $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
        }
    }).prop('disabled', !$.support.fileInput).parent().addClass($.support.fileInput ? undefined : 'disabled');

}
function DeleteGSTFile(fileNames, guid, OldInvoiceFile) {

    $.ajax(
        {
            url: urlDeleteGSTFile,
            data: { fileName: fileNames, FolderName: guid, OldInvoiceFile: OldInvoiceFile },
            contentType: 'application/json',
            method: 'get',
            async: false,
            success: function (data) {
                return false;
            },
        });
}
function DeleteGSTDocument(obj) {
    //var fileName = $(obj).attr('filename');
    var fileName = $(obj).attr('id');
    var FolderName = Model_Guid;
    var OldFileName = $(obj).attr('oldFileName');
    if (confirm('Are you sure you want to delete this file ?')) {
        $.ajax(
                {
                    url: urlDeleteGSTFile,
                    data: { fileName: fileName, FolderName: FolderName, OldInvoiceFile: OldFileName },
                    contentType: 'application/json',
                    method: 'get',
                    success: function () {

                        $("#tblDocuments").find('tr').each(function () {
                            $(this).remove();
                        });

                        $("#BasicDetails_GSTDocument").val('');

                        return false;
                    }
                });
    }
}
//Proof of Open upload
function OpenGSTDocument(obj) {
    var JobId = Model_Guid;

    //var fileName = $(obj).attr('filename');
    var fileName = $(obj).attr('id');
    var imagePath = UploadedDocumentPath + "/" + "JobDocuments" + "/" + JobId + "/" + "GST" + "/" + fileName;
    $("#loading-image").css("display", "");
    $('#imgViewImage').attr("src", imagePath).load(function () {
        logoWidthGST = this.width;
        logoHeightGST = this.height;
        $('#popupProof').modal({ backdrop: 'static', keyboard: false });

        if ($(window).height() <= logoHeightGST) {
            $("#imgViewImage").closest('div').height($(window).height() - 205);
            $("#imgViewImage").closest('div').css('overflow-y', 'scroll');
            $("#imgViewImage").height(logoHeightGST);
        }
        else {
            $("#imgViewImage").height(logoHeightGST);
            $("#imgViewImage").closest('div').removeAttr('style');
        }

        if (screen.width <= logoWidthGST || logoWidthGST >= screen.width - 250) {
            $('#popupProof').find(".modal-dialog").width(screen.width - 250);
            $("#imgViewImage").width(logoWidthGST);
        }
        else {
            $("#imgViewImage").width(logoWidthGST);
            $("#popupProof").find(".modal-dialog").width(logoWidthGST);
        }
        $("#loading-image").css("display", "none");
    });
    $("#imgViewImage").unbind("error");
    $('#imgViewImage').attr("src", imagePath).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });
}
function DownloadGSTDocument(obj) {
    var JobId = Model_Guid;
    var fileName = $(obj).attr('id');
    window.location.href = urlDownloadGSTDocument + fileName + '&FolderName=' + JobId;
}

function LoadImageSignature() {
    $("#loading-image").css("display", "");
    $('#imgAllSign').attr('src', SRCSignAll).load(function () {
        logoWidthAllSignature = this.width;
        logoHeightAllSignature = this.height;
        $('#popupSign').modal({ backdrop: 'static', keyboard: false });

        if ($(window).height() <= logoHeightAllSignature) {
            $("#imgAllSign").closest('div').height($(window).height() - 150);
            $("#imgAllSign").closest('div').css('overflow-y', 'scroll');
            $("#imgAllSign").height(logoHeightAllSignature);
        }
        else {
            $("#imgAllSign").height(logoHeightAllSignature);
            $("#imgAllSign").closest('div').removeAttr('style');
        }

        if (screen.width <= logoWidthAllSignature || logoWidthAllSignature >= screen.width - 250) {
            $('#popupSign').find(".modal-dialog").width(screen.width - 250);
            $("#imgAllSign").width(logoWidthAllSignature);
        }
        else {
            $("#imgAllSign").width(logoWidthAllSignature);
            $('#popupSign').find(".modal-dialog").width(logoWidthAllSignature);
        }
        $("#loading-image").css("display", "none");
    });
    $("#imgAllSign").unbind("error");
    $('#imgAllSign').attr('src', SRCSignAll).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });
}

function GetInstallerDesignerElectricianSignature(signType, installerDesignerElectricianId) {
    if (installerDesignerElectricianId > 0) {
        $.ajax(
        {
            url: urlGetInstallerDesignerElectricianSignature,
            data: { signatureTypeId: signType, jobId: BasicDetails_JobID, installerDesignerElectricianId: installerDesignerElectricianId },
            contentType: 'application/json',
            method: 'get',
            success: function (data) {
                if (data.status) {
                    if (signType == 2)
                        installerSignSRC = data.signature;
                    else if (signType == 4)
                        designerSignSRC = data.signature;
                    else if (signType == 3)
                        electricianSignSRC = data.signature;

                    if (signType == 2) {
                        $("#imgInstallerSignatureViewSWH").show();
                        $("#imgInstallerSignatureView").show();
                    }
                    else
                        $("#imgDesignerSignatureView").show();
                }
                else {
                    if (data.error)
                        showErrorMessagesForStatus(data.error);
                    else
                        showErrorMessagesForStatus('Signature does not exist.');
                }
            }
            , error: function () {
                showErrorMessagesForStatus('Signature does not exist.');
            }
        });
    }
    else {
        if (signType == 2) {
            $("#imgInstallerSignatureViewSWH").hide();
            $("#imgInstallerSignatureView").hide();
        }
        else
            $("#imgDesignerSignatureView").hide();
    }
}

function UploadAllSignatureOfJob(objSignUpload) {
    var typeOfSignature = objSignUpload.attr('typeOfSignature');

    objSignUpload.fileupload({       
        url: urlUploadJobSignature,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;
            for (var i = 0; i < data.result.length; i++) {

                 /* /JobDocuments/10233/sign(1).jpg */
                data.result[i].AbsolutePath = "/" + JobDocumentsToSavePath + "/" + BasicDetails_JobID + "/" + data.result[i].FileName;

                if (data.result[i].Status == true) {
                    if (typeOfSignature == 3) {
                        isElectricianSignUpload = true;
                        electricianSignSRC = data.result[i].AbsolutePath;
                        $("#imgElectricianSignatureView").show();

                    }
                    else if (typeOfSignature == 4) {
                        isDesignerSignUpload = true;
                        designerSignSRC = data.result[i].AbsolutePath;
                        $("#imgDesignerSignatureView").show();
                    }
                    else if (typeOfSignature == 2) {                    
                        isInstallerSignUpload = true;
                        installerSignSRC = data.result[i].AbsolutePath;
                        $("#imgInstallerSignatureViewSWH").show();
                        $("#imgInstallerSignatureView").show();
                    }
                    else if (typeOfSignature == 1) {
                        isOwnerSignUpload = true;
                        SRCOwnerSign = signatureURL + data.result[i].AbsolutePath;
                        $("#imgOwnerSignatureView").show();
                    }
                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName.replace("%", "$"));
                }
            }
            if (UploadFailedFiles.length > 0) {
                showErrorMessagesForStatus("File has not been uploaded.");
            }
            else {
                showSuccessMessages("File has been uploaded successfully.");
            }
        },
        progressall: function (e, data) {
        },

        singleFileUploads: false,
        send: function (e, data) {
            var documentType = data.files[0].type.split("/");
            var mimeType = documentType[0];
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(MaxImageSize_Job)) {
                        showErrorMessagesForStatus(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB");
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessagesForStatus("Please upload file that not conatain <> ^ [] .")
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(MaxImageSize_Job)) {
                    showErrorMessagesForStatus("Maximum file size limit exceeded.Please upload a file smaller than" + " " + maxsize + "MB");
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[0].name)) {
                    showErrorMessagesForStatus("Please upload file that not conatain <> ^ [] .")
                    return false;
                }
            }
            if (mimeType != "image") {
                showErrorMessagesForStatus("Please upload a file with .jpg , .jpeg or .png extension.");
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');
                return false;
            }
            $(".alert").hide();
            $("#errorMsgRegion").html("");
            $("#errorMsgRegion").hide();
            return true;
        },
        formData: { jobId: BasicDetails_JobID, typeOfSignature: typeOfSignature }
    });
}

function clearSignature(typeOfSignature) {
    if (confirm('Are you sure you want to delete uploaded signature?')) {
        if (typeOfSignature == 1) {
            ClearAllSignature(typeOfSignature, SRCOwnerSign, isOwnerSignUpload);
            isOwnerSignUpload = false;
            $("#imgOwnerSignatureView").hide();
        }
        else if (typeOfSignature == 2) {
            ClearAllSignature(typeOfSignature, installerSignSRC, isInstallerSignUpload);
            isInstallerSignUpload = false;
            $("#imgInstallerSignatureViewSWH").hide();
            $("#imgInstallerSignatureView").hide();
        }
        else if (typeOfSignature == 3) {
            ClearAllSignature(typeOfSignature, electricianSignSRC, isElectricianSignUpload);
            isElectricianSignUpload = false;
            $("#imgElectricianSignatureView").hide();
        }
        else if (typeOfSignature == 4) {
            ClearAllSignature(typeOfSignature, designerSignSRC, isDesignerSignUpload);
            isDesignerSignUpload = false;
            $("#imgDesignerSignatureView").hide();
        }
    }
}

function ClearAllSignature(typeOfSignature, signPath, isUpload) {
    $.ajax(
    {
        url: urlClearJobSignture,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        type: 'get',
        data: { typeOfSignature: typeOfSignature, signaturePath: signPath, jobId: BasicDetails_JobID, isUpload: isUpload.toString().toLowerCase() },
        success: function (response) {
            if (response.status) {
                showSuccessMessages("Signature has been deleted successfully.");
            }
            else
                showErrorMessagesForStatus(response.error);
        },
        error: function () {
            showErrorMessagesForStatus("Signature has not been deleted.");
        }
    });
}

function jobErrorMessage(message) {
    $("#errorMsgRegion").html(closeButton + message);
    $("#errorMsgRegion").show();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');
}

function checkExistSWHInstaller(obj, title) {
    var fieldName = obj.id;
    var chkvar = '';
    var uservalue = obj.value;
    var sID = 0;
    sID = company_Id;

    if (uservalue != "" && uservalue != undefined) {
        $.ajax(
             {
                 url: CheckSWHUserExistURL,
                 data: { UserValue: uservalue, FieldName: fieldName, userId: null, resellerID: null, solarCompanyId: parseInt(sID) },
                 contentType: 'application/json',
                 method: 'get',
                 success: function (data) {
                     if (data.status == false) {
                         var errorMsg;
                         errorMsg = data.message;
                         chkvar = false;

                         $(".alert").hide();
                         $("#errorMsgRegionSWHInstallerPopup").html(closeButton + errorMsg);
                         $("#errorMsgRegionSWHInstallerPopup").show();
                     }
                     else {
                         chkvar = true;
                     }
                 }
             });
    }
}

function SWHInstallerSignatureUpload() {
    var proofDocumentURL = UploadedDocumentPath;

    if ($('#imgSignSWHInstall').attr('src') != "") {
        if (JobInstallerDetailsSWH_SESignature != "") {
            var SignName = $('#imgSignSWHInstall').attr('src');
            var guid = ProjectSession_LoggedInUserId;
            var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
            var SRC = imagePath + "/" + SignName;
            SRCSignSWHInstall = SRC;
            $('#imgSignSWHInstall').attr('class', SignName);
            $('#imgSWHSignatureInstaller').show();
        }
    }

    if (BasicDetails_InstallerSignature != null && BasicDetails_InstallerSignature != '' && BasicDetails_InstallerSignature != undefined) {
        BasicDetails_InstallerSignature_Replace = BasicDetails_InstallerSignature.replace("\\", "/");
        $("#imgInstallerSign").attr('src', proofDocumentURL + BasicDetails_InstallerSignature_Replace + "?" + new Date().getTime());
    }
    else {
        $("#imgInstallerSign").attr('src', SRCSignSWHInstall);
    }


    $('#imgSWHSignatureInstaller').click(function () {    
        $("#loading-image").css("display", "");
        $('#imgSignSWHInstall').attr('src', SRCSignSWHInstall).load(function () {
            logoWidthSignSWHInstall = this.width;
            logoHeightSignSWHInstall = this.height;

            $('#popupboxSWHInsaller').modal({ backdrop: 'static', keyboard: false });

            if ($(window).height() <= logoHeightSignSWHInstall) {
                $("#imgSignSWHInstall").closest('div').height($(window).height() - 205);
                $("#imgSignSWHInstall").closest('div').css('overflow-y', 'scroll');
                $("#imgSignSWHInstall").height(logoHeightSignSWHInstall);
            }
            else {
                $("#imgSignSWHInstall").height(logoHeightSignSWHInstall);
                $("#imgSignSWHInstall").closest('div').removeAttr('style');
            }

            if (screen.width <= logoWidthSignSWHInstall || logoWidthSignSWHInstall >= screen.width - 250) {

                $('#popupboxSWHInsaller').find(".modal-dialog").width(screen.width - 250);
                $("#imgSignSWHInstall").width(logoWidthSignSWHInstall);
            }
            else {
                $("#imgSignSWHInstall").width(logoWidthSignSWHInstall);
                $('#popupboxSWHInsaller').find(".modal-dialog").width(logoWidthSignSWHInstall);
            }
            $("#loading-image").css("display", "none");
        });

        $("#popupboxSWHInsaller").unbind("error");

        $('#imgSignSWHInstall').attr('src', SRCSignSWHInstall).error(function () {
            alert('Image does not exist.');
            $("#loading-image").css("display", "none");
        });
    });

    $('#uploadBtnSignatureSWHInstaller').fileupload({
        url: Upload_User,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;

            var UploadFailedFilesName = [];
            UploadFailedFilesName.length = 0;
            //formbot start
            for (var i = 0; i < data.result.length; i++) {

                if (data.result[i].Status == true) {

                    var guid = ProjectSession_LoggedInUserId;

                    var signName = $('#imgSignSWHInstall').attr('class');
                    $("[name='JobInstallerDetails.SESignature']").each(function () {
                        $(this).remove();
                    });
                    if (signName != null && signName != "") {
                        DeleteFileFromLogoOnUpload(signName, guid, JobInstallerDetailsSWH_SESignature);
                    }
                    var proofDocumentURL = UploadedDocumentPath;
                    var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
                    SRCSignSWHInstall = imagePath + "/" + data.result[i].FileName;
                    $('#imgSignSWHInstall').attr('src', SRCSignSWHInstall);
                    $('#imgSignSWHInstall').attr('class', data.result[i].FileName);

                    $('#imgSWHSignatureInstaller').show();

                    $('<input type="hidden">').attr({
                        name: 'JobInstallerDetails.SESignature',
                        value: data.result[i].FileName,
                        id: data.result[i].FileName,
                    }).appendTo('form');

                }
                else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                    UploadFailedFilesName.push(data.result[i].FileName);
                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName);
                }
            }
            if (UploadFailedFiles.length > 0) {
                showErrorMessageForPopup(UploadFailedFiles.length + " " + "File has not been uploaded.", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
            }
            if (UploadFailedFilesName.length > 0) {
                showErrorMessageForPopup(UploadFailedFilesName.length + " " + "Uploaded filename is too big.", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
            }
            if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0) {
                showSuccessMessageForPopup("File has been uploaded successfully.", $("#successMsgRegionSWHInstallerPopup"), $("#errorMsgRegionSWHInstallerPopup"));
            }
        },

        progressall: function (e, data) {
        },

        singleFileUploads: false,
        send: function (e, data) {
            var documentType = data.files[0].type.split("/");
            var mimeType = documentType[0];
            if (data.files.length == 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].name.length > 50) {
                        showErrorMessageForPopup("Please upload small filename.", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessageForPopup("Please upload file that not conatain <> ^ [] .", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');
                        return false;
                    }
                }
            }
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(MaxLogoSize)) {
                        showErrorMessageForPopup(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxLogoSize + "MB", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessageForPopup("Please upload file that not conatain <> ^ [] .", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(MaxLogoSize)) {
                    showErrorMessageForPopup("Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxLogoSize + "MB", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
                    return false;
                }
            }
            if (mimeType != "image") {
                showErrorMessageForPopup("Please upload a file with .jpg , .jpeg or .png extension.", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');
                return false;
            }
            $(".alert").hide();
            $("#errorMsgRegion").html("");
            $("#errorMsgRegion").hide();
            // return true;
            $('<input type="hidden">').attr({
                name: 'Guid',
                value: ProjectSession_LoggedInUserId,
                id: ProjectSession_LoggedInUserId,
            }).appendTo('form');
            return true;
        },
        formData: { userId: ProjectSession_LoggedInUserId },
        change: function (e, data) {
            $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
        }
    }).prop('disabled', !$.support.fileInput)
.parent().addClass($.support.fileInput ? undefined : 'disabled');
}

$("#btnClosePopUpBoxSWHInsaller").click(function () {
    $('#popupboxSWHInsaller').modal('toggle');
});

function showErrorMessageForPopup(message, objError, objSuccess) {
    $(".alert").hide();
    if (objSuccess)
        objSuccess.hide();
    objError.html(closeButton + message);
    objError.show();
}

function showSuccessMessageForPopup(message, objSuccess, objError) {
    $(".alert").hide();
    if (objError)
        objError.hide();
    objSuccess.html(closeButton + message);
    objSuccess.show();
}