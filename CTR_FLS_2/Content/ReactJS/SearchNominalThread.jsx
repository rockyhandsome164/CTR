
const SearchNominalThread = (props) => {
    const [value, setValue] = React.useState(null);
    return (
        <>

            <div className="row">
                <div className="col-xs-6">
                    <div className="input-group">
                        <input type="text"
                            className="form-control"
                            placeholder="Search for DashNumber Or NominalThreadSize ..."
                            onChange={e => setValue(e.target.value)}
                        />
                        <span className="input-group-btn">
                            <button
                                type="submit"
                                value="Submit"
                                onClick={() => props.handleSearch(value)}
                                className="btn btn-success"
                            >
                                Search
                            </button>
                        </span>
                    </div>
                </div>
            </div>
        </>)
}
