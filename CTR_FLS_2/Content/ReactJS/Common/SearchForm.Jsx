const SearchForm = (props) => {
    const options = props.defaultType;
    const [searchData, setSearchData] = React.useState([]);
    const MasterSpecHeadings = ['Specification', 'Description', 'Master', 'DefaultType', 'DateEntered', 'EnteredBy', 'Action'];
    const [dataToSearch, setDataToSearch] = React.useState(
        {
            Specification: '',
            Master: '',
            DefaultTypeId: ''
        }
    );

    const handleOnChange = (event) => {
        const { name, value } = event.target;
        setDataToSearch({ ...dataToSearch, [name]: value });
    }

    const handleModalFormSubmit = (event) => {
        event.preventDefault();
        props.handleSearch(dataToSearch);
    }
    React.useEffect(() => {
        setSearchData(props);
    }, []);

    //Handles the  Select action on selecting table row from ItemsTable
    const selectMasterSpecTableRow = (selectedRow) => {
        setSearchData(selectedRow);
        props.handleSelectMasterSpecTableRow(selectedRow);
    }

    return (
        <>
            <div className="modal fade" id="AddEditModal" role="dialog" tabIndex="-1" aria-hidden="true" >
                <div className="modal-dialog modal-lg" style={{ width: "80%" }} role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h4 className="modal-title" >Search</h4>
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div className="modal-body">

                            <div className="row" style={{ marginTop: '5px' }}>
                                <form className="form-horizontal" onSubmit={handleModalFormSubmit}>
                                    <div className="form-group col-sm-3">
                                        <label htmlFor="Specification" className="col-sm-4 control-label">Specification:</label>
                                        <div className="col-sm-7">
                                            <input type="text"
                                                className="form-control"
                                                id="Specification"
                                                name="Specification"
                                                value={dataToSearch.Specification || ''}
                                                onChange={handleOnChange}
                                            />
                                        </div>
                                    </div>

                                    <div className="form-group col-sm-3">
                                        <label htmlFor="master" className="col-sm-4 control-label">Master:</label>
                                        <div className="col-sm-7">
                                            <input
                                                type="text"
                                                className="form-control"
                                                id="master"
                                                name="Master"
                                                value={dataToSearch.Master || ''}
                                                onChange={handleOnChange} />
                                        </div>
                                    </div>

                                    <div className="form-group col-sm-3">
                                        <label htmlFor="Type" className="col-sm-4 control-label">Type:</label>
                                        <div className="col-sm-7">
                                            <select
                                                className="form-control"
                                                id='defaulType'
                                                name="DefaultTypeId"
                                                value={dataToSearch.DefaultTypeId || ''}
                                                onChange={handleOnChange}
                                            >
                                                <option value="">Select an Option</option>
                                                {options.map((item, key) => (
                                                    <option
                                                        key={key}
                                                        value={item.Value}>
                                                        {item.Text}
                                                    </option>
                                                ))}
                                            </select>
                                        </div>
                                    </div>

                                    <div className="col-md-3 col-sm-5">
                                        <input
                                            id="Search"
                                            className="btn btn-success"
                                            type="submit"
                                            value={props.buttonlabel || 'Search'}
                                        />

                                        <button
                                            type="button"
                                            className="btn btn-default"
                                            data-dismiss="modal"
                                            style={{ marginLeft: "10px" }}
                                            onClick={() => props.afterCloseModal()}
                                        >
                                            Close
                                        </button>
                                    </div>

                                </form>
                            </div>

                            <div className="row" style={{ marginTop: '5px' }}>

                                <div className="container-fluid col-md-9" style={{ marginLeft: '130px' }} >

                                    <SharedTable
                                        headers={MasterSpecHeadings}
                                        rows={props.masterSpec}
                                        selectTableRow={selectMasterSpecTableRow}
                                        tableHeight={'200px'}
                                    />

                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </>
    )
}