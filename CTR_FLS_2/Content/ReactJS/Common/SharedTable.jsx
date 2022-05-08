const SharedTable = (props) => {
    const { headers, rows, selectTableRow } = props;

    const replaceCamelSpaces = (camelCase) => {
        return camelCase.replace(/\B([A-Z])\B/g, ' $1');
    }

    //CSS for Scrolling in Table 
    const tbody = {
        height: '250px',
        tableLayout: 'fixed',
        display: 'block',
        overflowY: 'auto',
        overflowX: 'hidden'
    };

    const tbody_tr = {
        display: 'table',
        width: '100%',
        tableLayout: 'fixed'
    }

    const thead = {
        display: 'table',
        width: '100%',
        tableLayout: 'fixed',
    }


    const getBuildRow = (row, value) => {

        switch (value) {
            case 'Action':
                return <button type="button" className="btn btn-success" data-dismiss="modal" onClick={() => handleSelect(row)} >Select</button>
            case 'Select':
                return <button type="button" className="btn btn-success" onClick={() => handleSelect(row)} >Select</button>
            default:
                return row[value];
        }

    };

    const buildRow = (row, headers) => {
        return (
            <tr key={row.Id} style={tbody_tr}>
                {headers.map((value, index) =>
                    <td key={index} >
                        {getBuildRow(row, value)}
                    </td>
                )}
            </tr>
        )
    };

    const handleSelect = (row) => {
        selectTableRow(row);
    }

    return (
        <>
            <div
                className={`row col-md-${props.tableColSize ? props.tableColSize : 10} col-sm-12`}
                style={{ marginTop: '5px' }}
            >
                <table className="table caption-top table-stripped table-hover" >
                    <thead style={thead}>
                        <tr className="active">
                            {headers.map(header => {
                                return (
                                    <th scope="col" key={header}>
                                        {replaceCamelSpaces(header)}
                                    </th>
                                );
                            })}
                        </tr>

                    </thead>

                    <tbody style={tbody}>
                        {rows && rows.map((value) => {
                            return buildRow(value, headers);
                        })}
                    </tbody>
                </table>
            </div>
        </>)
}