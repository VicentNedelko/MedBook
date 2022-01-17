

function FillInputs() {
    var actualBearingJson = document.getElementById('bearingIndicatorId').value;
    var bear = JSON.parse(actualBearingJson);
    console.log(bear);
    var unit = document.getElementById('indUnit');
    unit.value = bear['Unit'];
    var referenceMin = document.getElementById('indRefMin');
    var ref = bear['ReferenceMin'];
    console.log('Before - ', ref);
    var str = ref.toString();
    str = str.replace('.', ',');
    console.log('After - ', str);
    referenceMin.value = str;
    var referenceMax = document.getElementById('indRefMax');
    strMax = bear['ReferenceMax'].toString();
    strMax = strMax.replace('.', ',');
    referenceMax.value = strMax;
    console.log('RefMAX - ', strMax);
    var bearingIndId = document.getElementById('bearingId');
    bearingIndId.value = bear['Id'];

}

function FillUnitValue(i) {
    var actualIndicator = document.getElementById('indicators' + i).value;
    console.log('Indicator = ', actualIndicator);
    var indicator = JSON.parse(actualIndicator);
    var name = indicator['Name'];
    var unit = indicator['Unit'];
    var itemType = indicator['Type'];
    if (itemType == 0) {
        document.getElementById('indValueType' + i).innerHTML = '<input asp-for="@Model.Items[i].IndicatorValue" class="form - control" required />';
    }
    else {
        document.getElementById('indValueType' + i).innerHTML = '<select class="form-control" style="width:250px"><option value = "1">ВЫДЕЛЕНО</option><option value="0">Не обнаружено</option></select >';
    }
    document.getElementById('unitValue' + i).value = unit;
    document.getElementById('indName' + i).value = name;
}
