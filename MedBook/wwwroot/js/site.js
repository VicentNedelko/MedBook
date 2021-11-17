

function FillInputs() {
    var actualBearingJson = document.getElementById('bearingIndicatorId').value;
    var bear = JSON.parse(actualBearingJson);
    console.log(bear);
    var unit = document.getElementById('indUnit');
    unit.value = bear['Unit'];
    var referenceMin = document.getElementById('indRefMin');
    referenceMin.value = bear['ReferenceMin'];
    var referenceMax = document.getElementById('indRefMax');
    referenceMax.value = bear['ReferenceMax'];
    var bearingIndId = document.getElementById('bearingId');
    bearingIndId.value = bear['Id'];

}
