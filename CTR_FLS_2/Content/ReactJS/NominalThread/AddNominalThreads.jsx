
const AddNominalThread = (props) => {

    //All values of form will be stored in Array.
    const [values, setValues] = React.useState([]);

    // will handle the changes of form input paramters.
    const handleInputchange = (event) => {
        const { name, value } = event.target;     
        setValues({ ...values, [name]: value });
    }

    //get NominalTread By Id
    const getSingleNominalThread = (props) => {       
        (props.id) &&
            fetch('/nominalthread/getById?Id=' + props.id, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            })
                .then(response => response.json())
                .then(data => {
                    setValues(data);
                })
                .catch((error) => {
                    console.error('Error:', error);
                });
                
    }

    // will render the function on the changes of props.Id.
    React.useEffect(() => {
        getSingleNominalThread(props);
    }, [props]);

    // will handle the form submit event to submit form_data to parent component:
    const handleSubmit = (event) => {
        event.preventDefault();
        props.onSubmit(values);
        document.getElementById("CloseModal").click();
    }

    return (
        <>
            <div className="modal fade" id="AddEditModal" tabIndex="-1" aria-hidden="true" >
                <div className="modal-dialog">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h4 className="modal-title" >Add/Edit NominalThread</h4>
                        </div>
                        <div className="modal-body">
                            <form className="form-horizontal" onSubmit={handleSubmit}>
                                <div className="form-group ">
                                    <label htmlFor="DashNumber" className="col-sm-4 control-label">Dash Number</label>
                                    <div className="col-sm-8">
                                        <input
                                            type="text"
                                            className="form-control"
                                            id="DashNumber"
                                            name="DashNumber"
                                            value={values.DashNumber || ''}
                                            onChange={handleInputchange}
                                            required
                                            placeholder="Dash Number..."
                                        />
                                        <span className="help-block">
                                        </span>
                                    </div>
                                </div>
                                <div className="form-group">
                                    <label htmlFor="NominalThreadSize" className="col-sm-4 control-label">Nominal ThreadSize</label>
                                    <div className="col-sm-8">
                                        <input type="text"
                                            className="form-control"
                                            id="NominalThreadSize"
                                            name="NominalThreadSize"
                                            value={values.NominalThreadSize || ''}
                                            onChange={handleInputchange}
                                            required
                                            placeholder="Nominal ThreadSize..."
                                        />
                                    </div>
                                </div>
                                <div className="modal-footer">
                                    <button type="button" id="CloseModal" className="btn btn-default" onClick={() => setValues("")}
                                         data-dismiss="modal">Close</button>
                                    <button type="submit" value="Submit" className="btn btn-primary">Submit</button>
                                </div>
                        
                            </form>
                        </div>
                     
                    </div>
                </div>
            </div>
        </>)
}
