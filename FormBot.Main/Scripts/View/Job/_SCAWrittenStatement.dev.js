function AutoSign() {
	debugger;
	$('#AutoAssignment').modal('toggle');
	// $("#AutoAssignment").modal({ backdrop: 'static', keyboard: false });
}

function GetAutoSignSettingsData(userId, isForDefaultSelection = false) {
	$.ajax({
		url: urlGetAutoSignSettingsData,
		type: "GET",
		dataType: 'json',
		contentType: 'application/json; charset=utf-8', // Not to set any content header
		data: { UserId: userId, isForDefaultSelection: isForDefaultSelection },
		cache: false,
		success: function (result) {
			debugger;
			if (result.success) {
				if (localStorage.getItem(SolarCompanyId + "_Representative") == null) {
					$("#RetailerUserId option[value=" + result.data.RetailerUserId + "]").prop('selected', true);
					localStorage.setItem(SolarCompanyId + "_Representative", result.data.RetailerUserId);
				}
				else
					$("#RetailerUserId option[value=" + localStorage.getItem(SolarCompanyId + "_Representative") + "]").prop('selected', true);

				if (localStorage.getItem(SolarCompanyId + "_Signature") == null) {
					if (result.data.Signature != null && result.data.Signature != '' && result.data.Signature != undefined) {
						$("#imgRetailerSignature").attr('src', result.data.base64Img);
						localStorage.setItem(SolarCompanyId + "_Signature", result.data.base64Img);
					}
					else {
						$("#imgRetailerSignature").attr('src', "");
					}
				}
				else
					$("#imgRetailerSignature").attr('src', localStorage.getItem(SolarCompanyId + "_Signature"));

				$("#PositionHeld").val(result.data.PositionHeld);
				//var isEmployee = result.data.IsEmployee == true ? true : false;
				//var isSubContractor = result.data.IsSubContractor == true ? true : false;
				// var isChangedDesign = result.data.IsChangedDesign == true ? true : false;
				$("#IsEmployee option[value=" + result.data.IsEmployee + "]").prop('selected', true);
				//$('#IsSubContractor').prop('checked', isSubContractor);
				$("#IsChangedDesign option[value=" + result.data.IsChangedDesign + "]").prop('selected', true);
				//$('#IsChangedDesign').prop('checked', isChangedDesign);
				debugger;
				var modelRetailerSignURL = "UserDocuments/" + result.data.RetailerUserId + "/" + result.data.Signature;
				//if (result.data.Signature != null && result.data.Signature != '' && result.data.Signature != undefined) {
				//    $("#imgRetailerSignature").attr('src', signatureURL + modelRetailerSignURL);
				//}
				//else {
				//    $("#imgRetailerSignature").attr('src',"");
				//}
			}

			else {
				showErrorMessageForAutoSign("Something wrong happen");
			}

		}
	})
}

$("#RetailerUserId").change(function () {
	debugger;
	$.ajax({
		url: urlGetAutoSignSettingsData,
		type: "GET",
		dataType: 'json',
		contentType: 'application/json; charset=utf-8', // Not to set any content header
		data: { UserId: $('#RetailerUserId').val() },
		cache: false,
		success: function (result) {
			debugger;
			if (result.success) {
				if (result.data.RetailerUserId == 0) {
					$("#RetailerUserId").val('');
					$("#PositionHeld").val('');
					$("#IsEmployee").val('');

					$("#IsChangedDesign").val('');
					$("#imgRetailerSignature").attr("src", "")
				}
				else {
					$("#PositionHeld").val(result.data.PositionHeld);
					var isEmployee = result.data.IsEmployee == true ? true : false;
					var isSubContractor = result.data.IsSubContractor == true ? true : false;
					//$('#IsEmployee').prop('checked', isEmployee);
					//$('#IsSubContractor').prop('checked', isSubContractor);
					//var isChangedDesign = result.data.IsChangedDesign == true ? true : false;
					//$('#IsChangedDesign').prop('checked', isChangedDesign);
					$("#IsEmployee option[value=" + result.data.IsEmployee + "]").prop('selected', true);
					$("#IsChangedDesign option[value=" + result.data.IsChangedDesign + "]").prop('selected', true);
					debugger;
					var modelRetailerSignURL = "UserDocuments/" + result.data.RetailerUserId + "/" + result.data.Signature;
					if (result.data.Signature != null && result.data.Signature != '' && result.data.Signature != undefined) {
						$("#imgRetailerSignature").attr('src', result.data.base64Img);
						//localStorage.setItem(SolarCompanyId + "_Signature", signatureURL + modelRetailerSignURL);
					}
					else {
						$("#imgRetailerSignature").attr('src', "");
					}
					//if (localStorage.getItem(SolarCompanyId + "_Signature") == null) {

					//}
					//else
					//    $("#imgRetailerSignature").attr('src', localStorage.getItem(SolarCompanyId + "_Signature"));
				}

			}

			else {
				showErrorMessageForAutoSign("Something wrong happen");
			}

		}
	})

});

function getJobRetailerSettingDataForJobScreen(jobid) {
	$.ajax({
		url: urlGetJobRetailerSettingDataByJobId,
		type: "GET",
		dataType: 'json',
		contentType: 'application/json; charset=utf-8', // Not to set any content header
		data: { JobId: jobid },
		cache: false,
		success: function (result) {
			debugger;
			if (result.success) {
				//var modelRetailerSignURL = "UserDocuments/" + result.data.JobRetailerUserId + "/" + result.data.Signature;
				//if (result.data.Signature != null && result.data.Signature != '' && result.data.Signature != undefined) {
				//    $("#imgJobRetailerSignatureJobDetailScreen").attr('src', $("#imgJobRetailerSignature").attr('src'));
				//}
				//$("#RepresentativeName").val(result.data.RepresentativeName);
				//$("#JobRetailerPositionHeldlbl").val(result.data.PositionHeldlbl);
				$("#statement").html(result.data.Statement);
				//$("#SignedBy").text(result.data.SignedBy);
				//$("#SignedDate").text(result.data.SignedDate);
				//$("#JobRetailer_Latitude").text(result.data.Latitude);
				//$("#JobRetailer_Longitude").text(result.data.Longitude);

				// showSuccessMessageForJobAutoSign("Solar Representative data updated successfully.");


				//ReloadSCAWrittenStatement(JobId, SolarCompanyId, SolarCompanyName);
			}

			else {
				showErrorMessageForJobAutoSign("Something wrong happen");
			}

		}
	})
}

$("#JobRetailerUserId").change(function () {
	debugger;
	$.ajax({
		url: urlGetAutoSignSettingsDataRetailerIdWise,
		type: "GET",
		dataType: 'json',
		contentType: 'application/json; charset=utf-8', // Not to set any content header
		data: { JobId: JobId, SolarCompanyId: SolarCompanyId, userid: $("#JobRetailerUserId").val() },
		cache: false,
		success: function (result) {
			debugger;
			if (result.success) {
				if (result.data.JobRetailerUserId == 0) {
					$("#JobRetailerUserId").val('');
					$("#JobRetailerPositionHeld").val('');
					$("#JobWiseIsEmployee").val('');

					$("#JobWiseIsChangedDesign").val('');
					$("#imgJobRetailerSignature").attr("src", "")
				}
				else {
					$("#JobRetailerUserId option[value=" + result.data.JobRetailerUserId + "]").prop('selected', true);
					$("#JobRetailerPositionHeld").val(result.data.PositionHeld);
					//var isEmployee = result.data.IsEmployee == true ? true : false;
					//var isSubContractor = result.data.IsSubContractor == true ? true : false;
					// var isChangedDesign = result.data.IsChangedDesign == true ? true : false;
					$("#JobWiseIsEmployee option[value=" + result.data.IsEmployee + "]").prop('selected', true);
					//$('#IsSubContractor').prop('checked', isSubContractor);
					$("#JobWiseIsChangedDesign option[value=" + result.data.IsChangedDesign + "]").prop('selected', true);
					//$('#IsChangedDesign').prop('checked', isChangedDesign);
					debugger;
					$("#imgJobRetailerSignature").attr('src', result.data.base64Img);
					//var modelRetailerSignURL = "UserDocuments/" + result.data.JobRetailerUserId + "/" + result.data.Signature;
					//if (result.data.Signature != null && result.data.Signature != '' && result.data.Signature != undefined) {
					//    $("#imgJobRetailerSignature").attr('src', signatureURL + modelRetailerSignURL);
					//}
					//else {
					//    $("#imgJobRetailerSignature").attr('src', "");
					//}
				}
			}
			else {
				showErrorMessageForAutoSign("Something wrong happen");
			}

		}
	})

});

function SaveJobRetailerAutoSign() {
	debugger;
	var dataJobAutoSign = {};
	var UserId = $('#JobRetailerUserId').val();
	var Position = $("#JobRetailerPositionHeld").val();
	var isSubcontractor = $('#JobWiseIsEmployee').val() == "2" ? true : false;;
	//    var isEmployee = $('input:checkbox[id=IsEmployee]').is(':checked');
	//var isChangedDesign = $('input:checkbox[id=IsChangedDesign]').is(':checked');
	var isEmployee = $('#JobWiseIsEmployee').val() == "1" ? true : false;
	var isChangedDesign = $('#JobWiseIsChangedDesign').val() == "1" ? true : false;
	var StringjobRetailerBaseSignature = $("#imgJobRetailerSignature").attr('src');
	var Retailerbase30;
	var JobRetailerSignature;
	var JobRetailerIsUploaded;

	if (typeof (StringjobRetailerBaseSignature) == "undefined" || StringjobRetailerBaseSignature == "" || StringjobRetailerBaseSignature == null) {
		showErrorMessageForJobAutoSign("Please draw/upload signature.");
		return false;
	}
	JobRetailerSignature = JobId + "_RetailerSign" + "_" + UserId + "." + "png";
	JobRetailerIsUploaded = false;
	//if ($("#imgJobRetailerSignature").attr('isDraw') == 'false') {
	//    JobRetailerSignature = $('#imgJobRetailerSignature').attr('fileName');
	//    StringjobRetailerBaseSignature = "";
	//    JobRetailerIsUploaded = true;
	//}
	//else {
	//    if (isSignFromAutosetting == true) {
	//        StringjobRetailerBaseSignature = "";
	//        JobRetailerSignature = JobId + "_RetailerSign" + "_" + UserId + "." + "png";
	//        JobRetailerIsUploaded = true;
	//    }
	//    else {
	//        JobRetailerIsUploaded = false;
	//        JobRetailerSignature = JobId + "_RetailerSign" + "_" + UserId + "." + "png";
	//        AddRemoveSignatureLineForJobRetailer(true);
	//        StringjobRetailerBaseSignature = $('#popupJobAutoSignSignature #cSignature').jSignature('getData', 'image');
	//        Retailerbase30 = $('#popupJobAutoSignSignature #cSignature').jSignature('getData', 'base30');
	//        StringjobRetailerBaseSignature = StringjobRetailerBaseSignature.join(',');
	//    }

	//}
	$.validator.unobtrusive.parse("#frmJobAutoAssignment");
	if ($("#frmJobAutoAssignment").valid()) {
		dataJobAutoSign.UserId = UserId;
		dataJobAutoSign.Position = Position;
		dataJobAutoSign.isSubcontractor = isSubcontractor;
		dataJobAutoSign.isEmployee = isEmployee;
		dataJobAutoSign.isChangedDesign = isChangedDesign;
		dataJobAutoSign.StringOwnerBaseSignature = StringjobRetailerBaseSignature;
		dataJobAutoSign.IsUploaded = JobRetailerIsUploaded;
		dataJobAutoSign.Base30 = Retailerbase30;
		dataJobAutoSign.Signature = JobRetailerSignature;
		dataJobAutoSign.JobId = JobId;
		dataJobAutoSign.Latitude = $("#JobRetailerSetting_Latitude").val();
		dataJobAutoSign.Longitude = $("#JobRetailerSetting_Longitude").val();
		dataJobAutoSign.isSignFromAutosetting = isSignFromAutosetting;
		debugger;
		var SolarCompanyName = '@Model.BasicDetails.CompanyName';
		$.ajax({
			url: urlSaveJobAutoSignSettingsData,
			method: "post",
			contentType: 'application/json', // Not to set any content header
			data: JSON.stringify(dataJobAutoSign),
			success: function (result) {
				debugger;
				if (result.success) {
					var modelRetailerSignURL = "UserDocuments/" + result.data.JobRetailerUserId + "/" + result.data.Signature;
					if (result.data.Signature != null && result.data.Signature != '' && result.data.Signature != undefined) {
						$("#imgJobRetailerSignatureJobDetailScreen").attr('src', $("#imgJobRetailerSignature").attr('src'));
					}

					$("#RepresentativeName").val(result.data.RepresentativeName);
					$("#JobRetailerPositionHeldlbl").val(result.data.PositionHeldlbl);
					$("#statement").html(result.data.Statement);
					$("#SignedBy").text(result.data.SignedBy);
					$("#SignedDate").text(result.data.SignedDate);
					$("#JobRetailer_Latitude").text(result.data.Latitude);
					$("#JobRetailer_Longitude").text(result.data.Longitude);
					if (isFromJobScreen == false)
						showSuccessMessageForJobAutoSign("Solar Representative data updated successfully.");


					//ReloadSCAWrittenStatement(JobId, SolarCompanyId, SolarCompanyName);
				}

				else {
					showErrorMessageForJobAutoSign("Something wrong happen");
				}

			}
		})
	}
}

function validateFormAutoSign() {
	$('#JobRetailerPositionHeld').addClass("required");
	$("#JobRetailerPositionHeld").rules("add", {
		required: true,
		messages: {
			required: "Position is required."
		}
	});

}

function SaveAutoSignPopUp() {

	var dataAutoSign = {};
	var UserId = $('#RetailerUserId').val();
	var Position = $("#PositionHeld").val();
	var isSubcontractor = $("#IsEmployee").val() == "2" ? true : false;;
	//    var isEmployee = $('input:checkbox[id=IsEmployee]').is(':checked');
	//var isChangedDesign = $('input:checkbox[id=IsChangedDesign]').is(':checked');
	var isEmployee = $('#IsEmployee').val() == "1" ? true : false;
	var isChangedDesign = $('#IsChangedDesign').val() == "1" ? true : false;
	var StringOwnerBaseSignature = $('#imgRetailerSignature').attr('src');
	var base30;
	var RetailerSignature;
	var IsUploaded;
	if (typeof (StringOwnerBaseSignature) == "undefined") {
		showErrorMessageForAutoSign("Please draw/upload signature.");
		return false;
	}
	IsUploaded = false;
	RetailerSignature = "RetailerSign" + "_" + UserId + "." + "png";
	//    if ($("#imgRetailerSignature").attr('isDraw') == 'false') {
	//        RetailerSignature = $('#imgRetailerSignature').attr('fileName');
	//        StringOwnerBaseSignature = "";
	//        IsUploaded = true;
	//    }
	//    else {
	//        IsUploaded = false;
	//        RetailerSignature = "RetailerSign" + "_" + UserId + "." + "png";
	//        AddRemoveSignatureLine(true);
	//        StringOwnerBaseSignature = $('#popupAutoSignSignature #cSignature').jSignature('getData', 'image');
	//        base30 = $('#popupAutoSignSignature #cSignature').jSignature('getData', 'base30');

	//      //  RetailerSignature = OwnerSignature;
	//       StringOwnerBaseSignature = StringOwnerBaseSignature.join(',');
	//      // Base30 = base30;
	//}
	debugger;
	$.validator.unobtrusive.parse("#frmAutoAssignment");
	if ($("#frmAutoAssignment").valid()) {

		dataAutoSign.UserId = UserId;
		dataAutoSign.Position = Position;
		dataAutoSign.isSubcontractor = isSubcontractor;
		dataAutoSign.isEmployee = isEmployee;
		dataAutoSign.isChangedDesign = isChangedDesign;
		dataAutoSign.StringOwnerBaseSignature = StringOwnerBaseSignature;
		dataAutoSign.IsUploaded = IsUploaded;
		dataAutoSign.Base30 = base30;
		dataAutoSign.Signature = RetailerSignature;
		dataAutoSign.latitude = $("#JobRetailerSetting_Latitude").val();
		dataAutoSign.longitude = $("#JobRetailerSetting_Longitude").val();

		debugger;
		$.ajax({
			url: urlSaveAutoSignSettingsData,
			method: "post",
			contentType: 'application/json', // Not to set any content header
			data: JSON.stringify(dataAutoSign),
			success: function (result) {
				debugger;
				if (result.success) {
					//Setting up local storage for auto sign pop up representative dropdown selection
					debugger;
					localStorage.setItem(SolarCompanyId + "_Representative", $('#RetailerUserId').val());
					localStorage.setItem(SolarCompanyId + "_Signature", $('#imgRetailerSignature').attr('src'));
					showSuccessMessageForAutoSign("Solar Representative data updated successfully.");
				}

				else {
					showErrorMessageForAutoSign("Something wrong happen");
				}

			}
		})
	}
}

function SaveModalSignature() {
	debugger;
	$(".alert").hide();
	$("#errorMsgRegionSignPopup").hide();
	$("#successMsgRegionSignPopup").hide();

	if (($('#popupAutoSignSignature .signature-menu').find('.active').attr('isDraw') == "false") || $('#popupJobAutoSignSignature .signature-menu').find('.active').attr('isDraw') == "false") {

		debugger;
		if ($("#popupAutoSignSignature.in").length == 1) {
			var imgSrc = $("#popupAutoSignSignature #imgUploadSign").attr('src');
			if (typeof (imgSrc) == "undefined") {
				showErrorMessageForPopup("Please draw/upload signature.", $("#popupAutoSignSignature #errorMsgRegionSignPopup"), $("#popupAutoSignSignature #successMsgRegionSignPopup"));
				return false;
			}
			$("#imgRetailerSignature").attr('src', imgSrc);
			$("#imgRetailerSignature").attr('fileName', imgSrc.split(/[\\\/]/).pop());
			$("#imgRetailerSignature").attr('isDraw', false);
		} else {
			var imgJobSignSrc = $("#popupJobAutoSignSignature #imgUploadSign").attr('src');
			if (typeof (imgJobSignSrc) == "undefined") {
				showErrorMessageForPopup("Please draw/upload signature.", $("#popupAutoSignSignature #errorMsgRegionSignPopup"), $("#popupAutoSignSignature #successMsgRegionSignPopup"));
				return false;
			}
			$("#imgJobRetailerSignature").attr('src', imgJobSignSrc);
			$("#imgJobRetailerSignature").attr('fileName', imgJobSignSrc.split(/[\\\/]/).pop());
			$("#imgJobRetailerSignature").attr('isDraw', false);
		}

	}
	else {
		if ($("#popupAutoSignSignature.in").length == 1) {
			base30 = $('#popupAutoSignSignature #cSignature').jSignature('getData', 'base30');
			if (base30[1] == '') {
				showErrorMessageForPopup("Please draw/upload signature.", $("#popupAutoSignSignature #errorMsgRegionSignPopup"), $("#popupAutoSignSignature #successMsgRegionSignPopup"));
				return false;
			}
			AddRemoveSignatureLine(true);
			StringOwnerBaseSignature = $('#popupAutoSignSignature #cSignature').jSignature('getData', 'image');
			var imgSrc = StringOwnerBaseSignature.join(',');
			$("#imgRetailerSignature").attr('src', "data:" + imgSrc);
			$("#imgRetailerSignature").attr('isDraw', true);
			$('#popupAutoSignSignature').modal('toggle');



		}
		else {
			base30 = $('#popupJobAutoSignSignature #cSignature').jSignature('getData', 'base30');
			if (base30[1] == '') {
				showErrorMessageForPopup("Please draw/upload signature.", $("#popupJobAutoSignSignature #errorMsgRegionSignPopup"), $("#popupJobAutoSignSignature #successMsgRegionSignPopup"));
				return false;
			}
			AddRemoveSignatureLineForJobRetailer(true);
			StringOwnerBaseSignature = $('#popupJobAutoSignSignature #cSignature').jSignature('getData', 'image');
			var imgSrc = StringOwnerBaseSignature.join(',');
			$("#imgJobRetailerSignature").attr('src', "data:" + imgSrc);
			$("#imgJobRetailerSignature").attr('isDraw', true);
			$('#popupJobAutoSignSignature').modal('toggle');
		}
	}
	debugger;
	if (isFromJobScreen == true) {
		SaveJobRetailerAutoSign();
	}
}

function ClosePopupSignature() {
	if ($("#popupAutoSignSignature.in").length == 1) {
		if ($("#imgRetailerSignature").attr('isDraw') == 'false') {
			$('.signature-menu').find('.active').removeClass('active');
			$('.signature-menu').find('.primary').removeClass('primary');
			$('#aUploadSign').closest('li').addClass('active');
			$('#aUploadSign').closest('li').addClass('primary');
			$("#divUpload").show();
			$("#divDraw").hide();
		}
		else {
			$('.signature-menu').find('.active').removeClass('active');
			$('.signature-menu').find('.primary').removeClass('primary');
			$('#aDrawSign').closest('li').addClass('active');
			$('#aDrawSign').closest('li').addClass('primary');
			$("#divUpload").hide();
			$("#divDraw").show();
		}
		enableCompleteSigning($("#imgRetailerSignature").attr('src'));
		$('#popupAutoSignSignature').modal('toggle');
	}
	else {
		if ($("#imgJobRetailerSignature").attr('isDraw') == 'false') {
			$('.signature-menu').find('.active').removeClass('active');
			$('.signature-menu').find('.primary').removeClass('primary');
			$('#aUploadSign').closest('li').addClass('active');
			$('#aUploadSign').closest('li').addClass('primary');
			$("#divUpload").show();
			$("#divDraw").hide();
		}
		else {
			$('.signature-menu').find('.active').removeClass('active');
			$('.signature-menu').find('.primary').removeClass('primary');
			$('#aDrawSign').closest('li').addClass('active');
			$('#aDrawSign').closest('li').addClass('primary');
			$("#divUpload").hide();
			$("#divDraw").show();
		}
		enableCompleteSigning($("#imgJobRetailerSignature").attr('src'));
		$('#popupJobAutoSignSignature').modal('toggle');
	}

}

function SignatureInAutoSetting() {
	if ($('#RetailerUserId').val() != '' && $('#RetailerUserId').val() != 0) {
		$("#errorMsgRegionSignPopup").hide();
		$("#successMsgRegionSignPopup").hide();
		AddRemoveSignatureLine(false);
		$('#popupAutoSignSignature').modal({ backdrop: 'static', keyboard: false });
	}
	else {
		showErrorMessageForAutoSign("Please select retailer first for signature.");
	}

}

function SignatureInJobAutoSetting() {
	debugger;
	if ($('#JobRetailerUserId').val() != '' && $('#JobRetailerUserId').val() != 0) {
		$("#errorMsgRegionSignPopup").hide();
		$("#successMsgRegionSignPopup").hide();
		AddRemoveSignatureLineForJobRetailer(false);
		isSignFromAutosetting = false;
		$('#popupJobAutoSignSignature').modal({ backdrop: 'static', keyboard: false });
	}
	else {
		showErrorMessageForAutoSign("Please select retailer first for signature.");
	}

}

function SignatureInJobAutoSettingFromJobScreen() {
	debugger;
	if ($('#JobRetailerUserId').val() != '' && $('#JobRetailerUserId').val() != 0) {
		$("#errorMsgRegionSignPopup").hide();
		$("#successMsgRegionSignPopup").hide();
		AddRemoveSignatureLineForJobRetailer(false);
		isSignFromAutosetting = false;
		isFromJobScreen = true;
		$('#popupJobAutoSignSignature').modal({ backdrop: 'static', keyboard: false });
	}
	else {
		showErrorMessageForAutoSign("Please select retailer first for signature.");
	}

}

function SignatureInJobFromAutoSetting() {
	if ($('#JobRetailerUserId').val() != '' && $('#JobRetailerUserId').val() != 0) {
		$.ajax({
			url: urlGetSignFromAutoSetting,
			type: "GET",
			dataType: 'json',
			contentType: 'application/json; charset=utf-8', // Not to set any content header
			data: {
				userid: $("#JobRetailerUserId").val()
			},
			cache: false,
			success: function (result) {
				debugger;
				if (result.success) {
					var retailerId = $("#JobRetailerUserId").val();
					var modelRetailerSignURL = "UserDocuments/" + retailerId + "/" + result.data;
					if (result.data != null && result.data != '' && result.data != undefined) {
						$("#imgJobRetailerSignature").attr('src', result.data);
						//$("#imgJobRetailerSignature").attr('src', signatureURL + modelRetailerSignURL);
						isSignFromAutosetting = true;
						// $("#imgJobRetailerSignature").attr('isDraw', "false");
						// AddRemoveSignatureLineForJobRetailer(true);
					}
				}

				else {
					showErrorMessageForAutoSign("Something wrong happen");
				}

			}
		})
	}
	else {
		showErrorMessageForAutoSign("Please select retailer first for signature.");
	}

}

function AddRemoveSignatureLine(isRemove) {
	if (isRemove) {
		$('#popupAutoSignSignature #cSignature').find("canvas.jSignature").add($('#popupAutoSignSignature #cSignature').filter("canvas.jSignature")).data("jSignature.this").settings['background-color'] = 'transparent';
		$('#popupAutoSignSignature #cSignature').find("canvas.jSignature").add($('#popupAutoSignSignature #cSignature').filter("canvas.jSignature")).data("jSignature.this").settings['decor-color'] = 'transparent'
	}
	else {
		$('#popupAutoSignSignature #cSignature').find("canvas.jSignature").add($('#popupAutoSignSignature #cSignature').filter("canvas.jSignature")).data("jSignature.this").settings['background-color'] = 'rgb(255, 255, 255)';
		$('#popupAutoSignSignature #cSignature').find("canvas.jSignature").add($('#popupAutoSignSignature #cSignature').filter("canvas.jSignature")).data("jSignature.this").settings['decor-color'] = 'rgb(134, 134, 134)';
	}
	$('#popupAutoSignSignature #cSignature').find("canvas.jSignature").add($('#popupAutoSignSignature #cSignature').filter("canvas.jSignature")).data("jSignature.this").resetCanvas($('#popupAutoSignSignature #cSignature').find("canvas.jSignature").add($('#popupAutoSignSignature #cSignature').filter("canvas.jSignature")).data("jSignature.this").settings['data']);
}

function AddRemoveSignatureLineForJobRetailer(isRemove) {
	if (isRemove) {
		$('#popupJobAutoSignSignature #cSignature').find("canvas.jSignature").add($('#popupJobAutoSignSignature #cSignature').filter("canvas.jSignature")).data("jSignature.this").settings['background-color'] = 'transparent';
		$('#popupJobAutoSignSignature #cSignature').find("canvas.jSignature").add($('#popupJobAutoSignSignature #cSignature').filter("canvas.jSignature")).data("jSignature.this").settings['decor-color'] = 'transparent'
	}
	else {
		$('#popupJobAutoSignSignature #cSignature').find("canvas.jSignature").add($('#popupJobAutoSignSignature #cSignature').filter("canvas.jSignature")).data("jSignature.this").settings['background-color'] = 'rgb(255, 255, 255)';
		$('#popupJobAutoSignSignature #cSignature').find("canvas.jSignature").add($('#popupJobAutoSignSignature #cSignature').filter("canvas.jSignature")).data("jSignature.this").settings['decor-color'] = 'rgb(134, 134, 134)';
	}
	$('#popupJobAutoSignSignature #cSignature').find("canvas.jSignature").add($('#popupJobAutoSignSignature #cSignature').filter("canvas.jSignature")).data("jSignature.this").resetCanvas($('#popupJobAutoSignSignature #cSignature').find("canvas.jSignature").add($('#popupJobAutoSignSignature #cSignature').filter("canvas.jSignature")).data("jSignature.this").settings['data']);
}

function showSuccessMessageForAutoSign(message) {
	$(".alert").hide();
	$("#errorMsgRegionAutoSetting").hide();
	$("#successMsgRegionAutoSetting").html(closeButton + message);
	$("#successMsgRegionAutoSetting").show();
}

function showErrorMessageForAutoSign(message) {
	$(".alert").hide();
	$("#successMsgRegionAutoSetting").hide();
	$("#errorMsgRegionAutoSetting").html(closeButton + message);
	$("#errorMsgRegionAutoSetting").show();
}

function showSuccessMessageForJobAutoSign(message) {
	$(".alert").hide();
	$("#errorMsgRegionSCAStatement").hide();
	$("#successMsgRegionSCAStatement").html(closeButton + message);
	$("#successMsgRegionSCAStatement").show();
}

function showErrorMessageForJobAutoSign(message) {
	$(".alert").hide();
	$("#successMsgRegionSCAStatement").hide();
	$("#errorMsgRegionSCAStatement").html(closeButton + message);
	$("#errorMsgRegionSCAStatement").show();
}

function showSuccessMessageForJobScreen(message) {
	$(".alert").hide();
	$("#errorMsgRegionSCAStatementJobScreen").hide();
	$("#successMsgRegionSCAStatementJobScreen").html(closeButton + message);
	$("#successMsgRegionSCAStatementJobScreen").show();
}

function showErrorMessageForJobScreen(message) {
	$(".alert").hide();
	$("#successMsgRegionSCAStatementJobScreen").hide();
	$("#errorMsgRegionSCAStatementJobScreen").html(closeButton + message);
	$("#errorMsgRegionSCAStatementJobScreen").show();
}

function enableCompleteSigning(obj) {
	if (typeof (obj) != "undefined") {
		$('#btnSaveOwnerSignature').removeClass('default');
		$('#btnSaveOwnerSignature').addClass('primary');
	}
	else {
		$('#btnSaveOwnerSignature').removeClass('primary');
		$('#btnSaveOwnerSignature').addClass('default');
	}
}

function showPosition(position) {
	debugger;
	//x.innerHTML = "Latitude: " + position.coords.latitude +
	//"<br>Longitude: " + position.coords.longitude;
	$("#JobRetailerSetting_Latitude").val(position.coords.latitude);
	$("#JobRetailerSetting_Longitude").val(position.coords.longitude);
	console.log(position.coords.latitude + " : " + position.coords.longitude)
	GetAddress(position.coords.latitude, position.coords.longitude);
}

function GetAddress(lat, lng) {
	var address = '';
	var latlng = new google.maps.LatLng(lat, lng);
	var geocoder = geocoder = new google.maps.Geocoder();
	geocoder.geocode({ 'latLng': latlng }, function (results, status) {
		if (status == google.maps.GeocoderStatus.OK) {
			if (results[1]) {
				address = results[1].formatted_address;
				$("#JobOwnerDetails_Location").val(results[1].formatted_address);
				//x.innerHTML = results[1].formatted_address;
			}
		}
	});
	return address;
}

function getLocation() {
	debugger;
	if (navigator.geolocation) {
		navigator.geolocation.getCurrentPosition(showPosition, errorHandler, { timeout: 500 });
	} else {
		//x.innerHTML = "Geolocation is not supported by this browser.";
	}
}

function errorHandler(error) {
	debugger;
	if (error.code == 1) {
		console.log("Error: Access is denied!");
	} else if (error.code == 2) {
		console.log("Error: Position is unavailable!");
	}
}

function ReloadSCAWrittenStatement(jobId, solarCompanyId, CompanyName) {
	$("#loading-image").show();
	setTimeout(function () {
		$.get('@Url.Action("GetSCAWrittenStatement", "job")?jobId=' + jobId + '&solarCompanyId=' + solarCompanyId + '&companyName=' + CompanyName, function (data) {
			$('#reloadRetailerAutoSetting').empty();
			$('#reloadRetailerAutoSetting').append(data);

			$("#loading-image").hide();
		});
	}, 500);
}

function SetDataFromAutoSignSetting() {
	$.ajax({
		url: urlSetAutoSignSettingsDataInJob,
		type: "GET",
		dataType: 'json',
		contentType: 'application/json; charset=utf-8', // Not to set any content header
		data: {
			JobId: JobId, SolarCompanyId: SolarCompanyId, UserId: $('#RetailerUserId').val()
		},
		cache: false,
		success: function (result) {
			debugger;
			if (result.success) {
				if (result.data.Signature != null && result.data.Signature != '' && result.data.Signature != undefined) {
					$("#JobRetailerUserId option[value=" + result.data.JobRetailerUserId + "]").prop('selected', true);
					$("#JobRetailerPositionHeld").val(result.data.PositionHeld);
					//var isEmployee = result.data.IsEmployee == true ? true : false;
					//var isSubContractor = result.data.IsSubContractor == true ? true : false;
					// var isChangedDesign = result.data.IsChangedDesign == true ? true : false;
					$("#JobWiseIsEmployee option[value=" + result.data.IsEmployee + "]").prop('selected', true);
					//$('#IsSubContractor').prop('checked', isSubContractor);
					$("#JobWiseIsChangedDesign option[value=" + result.data.IsChangedDesign + "]").prop('selected', true);
					//$('#IsChangedDesign').prop('checked', isChangedDesign);
					$("#RepresentativeName").val(result.data.RepresentativeName);
					$("#JobRetailerPositionHeldlbl").val(result.data.PositionHeldlbl);
					$("#statement").html(result.data.Statement);
					$("#SignedBy").text(result.data.SignedBy);
					$("#SignedDate").text(result.data.SignedDate);
					$("#JobRetailer_Latitude").text(result.data.Latitude);
					$("#JobRetailer_Longitude").text(result.data.Longitude);

					var modelRetailerSignURL = "UserDocuments/" + result.data.JobRetailerUserId + "/" + result.data.Signature;
					$("#imgJobRetailerSignatureJobDetailScreen").attr('src', result.data.base64Img);
					$("#imgJobRetailerSignature").attr('src', result.data.base64Img);

					// $("#imgJobRetailerSignatureJobDetailScreen").attr('src', signatureURL + modelRetailerSignURL);
					showSuccessMessageForJobScreen("Represenatative data updated successfully from auto sign setting.");
				}
				else {
					showErrorMessageForJobScreen("Auto Sign Template is not proper,signature required in auto sign setting template");
				}
			}

			else {
				showErrorMessageForJobScreen("Auto Sign Template is not proper,signature required in auto sign setting template");
			}


		}
	})

}