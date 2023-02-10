function html_to_string(html)
    {
        return decodeURIComponent(html);
}
$.ajax
    ({
        url: file,
        dataType: 'json',
        success: function (data) {
            displayTree(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error(errorThrown);
        }
    });