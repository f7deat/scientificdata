function exportTopic() {
    $.ajax({
        url: '/admin/tools/ExportTopic',
        type: 'GET',
        dataType: 'html'
    }).done(function (response) {
        if (response.length > 0) {

            let csv = ConvertToCSV(response);
            var link = window.document.createElement("a");
            link.setAttribute("href", "data:text/csv;charset=utf-8,%EF%BB%BF" + encodeURI(csv));
            link.setAttribute("download", "my_data.csv");
            link.click(); 
        }
        else {
            toastr["error"]("Export thất bại!");
        }
    });
}
// JSON to CSV Converter
function ConvertToCSV(objArray) {
    var array = typeof objArray !== 'object' ? JSON.parse(objArray) : objArray;
    var str = '';

    for (var i = 0; i < array.length; i++) {
        var line = '';
        for (var index in array[i]) {
            if (line !== '') line += ',';

            line += array[i][index];
        }

        str += line + '\r\n';
    }

    return str;
}