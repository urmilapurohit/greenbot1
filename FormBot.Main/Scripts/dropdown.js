var _keys = [];
var _source = [];
var others = [];
var aThis = [];

Array.prototype.bindDropdown = function () {
    var _this = this,
        _data = [];
    _this.forEach(function (e, i, a) {
        if (e.key) {
            if (_keys.indexOf(e.key) < 0) {
                _keys.push(e.key);
                _data.push({ id: e.key, proc: e.proc, parameters: e.param || [] });
            }
        }
        else {
            _data.push({ id: e.id, proc: e.proc, parameters: e.param || [] });
        }      
        aThis.push(e);      
    });
    var callback = function (data) {
        if (data) {
            aThis.forEach(function (e, i, a) {
                if (!e.bind) {
                    var dataObj = _source.filter(a=>a.id == (e.key || e.id));
                    if (dataObj.length > 0) {
                        e.bind = true;
                        FillDropDown2(e.id, dataObj[0].data, e.value, e.hasSelect, e.callback, e.defaultText, e.bText, e.bValue);
                    }
                    else {
                        if (data[e.key || e.id]) {
                            _source.push({ id: e.key || e.id, data: data[e.key || e.id] });
                            e.bind = true;
                            FillDropDown2(e.id, data[e.key || e.id], e.value, e.hasSelect, e.callback, e.defaultText, e.bText, e.bValue);
                        }
                    }
                }
            });
        }
        
    }
    var data = _data.getData(callback);
}

Array.prototype.getData = function (callback) {
    var _this = this;
    if (_this.length > 0) {
        $.ajax({
            type: "POST",
            url: URLJobData,
            data: JSON.stringify(_this),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                if (data.success) {
                    if (callback) {
                        callback(JSON.parse(data.data))
                    }
                }
            }
        });
    }
}

function FillDropDown2(id, data, value, hasSelect, callback, defaultText,bText,bValue) {


    var options = '';
    if (hasSelect == true) {
        if (defaultText == undefined || defaultText == null)
            defaultText = 'Select';

        options = "<option value=''>" + defaultText + "</option>";
    }

    $.each(data, function (i, val) {
        options += '<option value = "' + val[bValue] + '" >' + val[bText] + '</option>'
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

    if (callback) {
        callback();
        //setDropDownWidth(id);
    }


}