

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

function FillDoctorId() {
    const id = document.getElementById('docIdSelector').value;
    console.log('DocID = ' + id);
    const idDocInput = document.getElementById('doctorId');
    idDocInput.value = id;
    console.log('DOC Done!');
}

function FillPatientId() {
    const id = document.getElementById('patIdSelector').value;
    console.log('PatID = ' + id);
    const idPatInput = document.getElementById('patientId');
    idPatInput.value = id;
    console.log('PAT Done!');
}

function SetEndTime() {
    const startTime = document.getElementById('startTime').value;
    console.log('Start time = ' + startTime);
    const endTimeTag = document.getElementById('endTime');
    const endTimeValue = new Date(startTime);
    console.log('End Time Value = ' + endTimeValue);
    endTimeValue.setMinutes(endTimeValue.getMinutes() + 20);
    console.log('EndTime Added = ' + endTimeValue);
    var Year = endTimeValue.getFullYear();
    var Mon = endTimeValue.getMonth();
    if (Mon < 10) { var MonStr = '0' + Mon.toString() }
    else { var MonStr = Mon.toString() };
    var Day = endTimeValue.getDate();
    if (Day < 10) { var DayStr = '0' + Day.toString() }
    else { var DayStr = Day.toString() };
    var Hour = endTimeValue.getHours();
    if (Hour < 10) { var HourStr = '0' + Hour.toString() }
    else { var HourStr = Hour.toString() };
    var Min = endTimeValue.getMinutes();
    if (Min < 10) { var MinStr = '0' + Min.toString() }
    else { var MinStr = Min.toString() };
    endTimeTag.value = Year.toString() + '-' + MonStr + '-' + DayStr + 'T' + HourStr + ':' + MinStr;
}
