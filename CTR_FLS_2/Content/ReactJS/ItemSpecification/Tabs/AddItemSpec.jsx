
const AddItemSpec = (props) => {

    //All values of form will be stored in Array.
    const [values, setValues] = React.useState([]);
    const [isError, setIsError] = React.useState([]);

    const validateFields = () => {
        let isError = false;
        const errors = [];

        if (!values.Description) {
            setIsError(true);
            errors.push('description required');
        }
        if (!values.Item) {
            setIsError(true);
            errors.push('Item required');
        }
        if (!values.Spec) {
            setIsError(true);
            errors.push('Spec required');
        }

        return isError;
    }

    // will handle the changes of form input paramters.
    const handleInputchange = (event) => {
        const { name, value } = event.target;
        setValues({ ...values, [name]: value });
    }

    //get NominalTread By Id
    const getSingleITemSpec = (props) => {
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
    //React.useEffect(() => {
    //    getSingleITemSpec(props);
    //}, [props]);

    // will handle the form submit event to submit form_data to parent component:
    const handleSubmit = (event) => {
        event.preventDefault();
        props.onSubmit(values);
    }

    return (
        <>

            <form className="form-horizontal" onSubmit={handleSubmit}>
                <div className="form-group ">
                    <label htmlFor="Description" className="col-sm-4 control-label">Description:</label>
                    <div className="col-sm-8">
                        <input
                            type="text"
                            className="form-control"
                            id="Description"
                            name="Description"
                            value={values.Description || ''}
                            onChange={handleInputchange}                           
                            placeholder="Description..."
                        />
                        
                    </div>
                </div>
                <div className="form-group">
                    <label htmlFor="Item" className="col-sm-4 control-label">Item:</label>
                    <div className="col-sm-8">
                        <input type="text"
                            className="form-control"
                            id="Item"
                            name="Item"
                            value={values.Item || ''}
                            onChange={handleInputchange}                           
                            placeholder="Item..."
                        />
                    </div>
                </div>

                <div className="form-group">
                    <label htmlFor="ItemType" className="col-sm-4 control-label">ItemType:</label>
                    <div className="col-sm-8">
                        <input type="text"
                            className="form-control"
                            id="ItemType"
                            name="ItemType"
                            value={values.ItemType || ''}
                            onChange={handleInputchange}                           
                            placeholder="ItemType..."
                        />
                    </div>
                </div>

                <div className="form-group">
                    <label htmlFor="Spec" className="col-sm-4 control-label">Specification:</label>
                    <div className="col-sm-8">
                        <input type="text"
                            className="form-control"
                            id="Spec"
                            name="Spec"
                            value={values.Spec || ''}
                            onChange={handleInputchange}
                            placeholder="Specification..."
                        />
                    </div>
                </div>

                <div className="modal-footer">
                    <button type="button" id="CloseModal" className="btn btn-default"
                        data-dismiss="modal" onClick={() => props.afterCloseModal()} >Close</button>
                    <button type="submit" value="Submit" className="btn btn-primary">Submit</button>
                </div>

            </form>

        </>)
}
