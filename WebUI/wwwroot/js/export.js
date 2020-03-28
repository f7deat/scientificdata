function convertToCSV(objArray) {
    var array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
    var str = '';

    for (var i = 0; i < array.length; i++) {
        var line = '';
        for (var index in array[i]) {
            if (line != '') line += ','

            line += array[i][index];
        }

        str += line + '\r\n';
    }

    return str;
}

function exportCSVFile(headers, items, fileTitle) {
    if (headers) {
        items.unshift(headers);
    }

    // Convert Object to JSON
    var jsonObject = JSON.stringify(items);

    var csv = this.convertToCSV(jsonObject);

    var exportedFilenmae = fileTitle + '.csv' || 'export.csv';

    var blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    if (navigator.msSaveBlob) { // IE 10+
        navigator.msSaveBlob(blob, exportedFilenmae);
    } else {
        var link = document.createElement("a");
        if (link.download !== undefined) { // feature detection
            // Browsers that support HTML5 download attribute
            var url = URL.createObjectURL(blob);
            link.setAttribute("href", url);
            link.setAttribute("download", exportedFilenmae);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    }
}

function download() {
    var headers = {
        topicId: 'Id'.replace(/,/g, ''), // remove commas to avoid errors
        name: "Tên",
        url: "Url",
        categoryId: "Danh Mục",
        description: "Tóm tắt",
        content: "Nội dung",
        createdDate: "Ngày tạo",
        modifiedDate: "Ngày cập nhật",
        publishDate: "Ngày hiệu lực",
        userId: "Tài khoản",
        status: "Trạng thái",
        attachments: "Earphones",
        tags: "Earphones",
        source: "Earphones",
        signer: "Earphones",
        topicType: "Earphones",
        effectiveDate: "Earphones",
        number: "Earphones",
        departmentId: "Earphones",
        attachmentType: "Earphones",
        page: "Earphones",
        issn: "Earphones",
        warehouseId: "Earphones"
    };

    itemsNotFormatted = [];

    $.ajax({
        url: '/admin/tools/exporttopic',
        type: 'GET',
        dataType: 'html',
        data: {}
    }).done(function (response) {
        itemsNotFormatted = JSON.parse(response);
        var itemsFormatted = [];

        // format the data
        itemsNotFormatted.forEach((item) => {
            itemsFormatted.push({
                topicId: item.topicId,
                name: item.name,
                url: item.url,
                categoryId: item.categoryId,
                description: item.description,
                content: item.content,
                createdDate: item.createdDate,
                modifiedDate: item.modifiedDate,
                publishDate: item.publishDate,
                userId: item.userId,
                status: item.status,
                attachments: item.attachments,
                tags: item.tags,
                source: item.source,
                signer: item.signer,
                topicType: item.topicType,
                effectiveDate: item.effectiveDate,
                number: item.number,
                departmentId: item.departmentId,
                attachmentType: item.attachmentType,
                page: item.page,
                issn: item.issn,
                warehouseId: item.warehouseId
            });
        });

        var fileTitle = 'topics'; // or 'my-unique-title'

        exportCSVFile(headers, itemsFormatted, fileTitle); // call the exportCSVFile() function to process the JSON and trigger the download
    });

}