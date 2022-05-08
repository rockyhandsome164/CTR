
const SharedForm = (props) => {
    const [searchData, setSearchData] = React.useState([]);
    const [value, setValue] = React.useState();

    const handleOnChange = (e) => {
        setValue(e.target.value);
        setSearchData(() => e.target.value);
    }

    const handleModalFormSubmit = (event) => {
        event.preventDefault();
        props.handleSearch(searchData);
    }

    React.useEffect(() => {
        setSearchData([]);
    },
        // it means it will be rerendered every time props.value changes
        []
    );

    //it gets initialized with the component
    React.useEffect(() => {
        setSearchData([]);
        if (props.value) {
            setValue(props.value);
        }
        else {
            setValue(() => "");
        }
    },
        // it means it will be rerendered every time props.value changes
        [props.value]

    );
    /* className = "form-horizontal col-md-6"*/
    return (
        <>
            <div className="row" style={{ marginTop: '5px' }}>
                <form className={`form-horizontal col-md-${props.formColSize ? props.formColSize : 6} `}
                    onSubmit={handleModalFormSubmit}>
                    <div className="form-group ">
                        <label htmlFor={props.id} className="col-sm-2 control-label">{props.label}:</label>

                        <div className="col-sm-5">
                            {props.Type == 'textarea' ?
                                <textarea
                                    className="form-control"
                                    value={value}
                                    id={props.id}
                                    rows='10'
                                    placeholder='Type to add a note...'
                                    onChange={handleOnChange}
                                ></textarea> :
                                <input
                                    type='text'
                                    className="form-control"
                                    id={props.id}
                                    placeholder={props.placeholder || 'Enter to Search...'}
                                    onChange={handleOnChange}
                                    value={searchData}
                                    autoComplete="off" />
                            }

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
                            >
                                Close
                            </button>
                        </div>
                    </div>
                </form>
            </div>

            <div className="row" style={{ marginTop: '5px', marginLeft: '5px' }}>

                <div className="row" style={{ marginTop: '5px', marginLeft: '5px' }}>
                    {props.searchTable || null}
                </div>
            </div>
        </>
    )
}