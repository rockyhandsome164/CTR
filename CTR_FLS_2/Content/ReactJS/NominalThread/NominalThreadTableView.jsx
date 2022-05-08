
const NominalThreadTableView = () => {
    // nominalThreadData: to store the data from database for get function
    const [nominalThreadData, setnominalThreadData] = React.useState([]);

    // is for Modal control
    const [IsModalOpen, setIsModalOpen] = React.useState(false);

    // will store the state of Id for editing nominalthread data.
    const [nominalThreadId, setNominalThreadId] = React.useState();

    //get AllNominalTread Data and if have search paramters then only searched data.
    const getNominalThreads = (searchParams) => {
        
        fetch(`/nominalthread/getallnominalthreads?searchParams=${searchParams}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        })
            .then(response => response.json())
            .then(data => {
                setnominalThreadData(data);
            })
            .catch((error) => {
                console.error('Error from getNominalThreads:', error);
            });
    }

    // will call the fxn: getNominalThreads when component is rendered.
    React.useEffect(() => {
        getNominalThreads("");
    }, []);

    //Form submission function
    //This function is called from AddNominalThread Component after clicking submit button
    const addNewNominalThread = (nominalData) => {
        console.log("DatatoPost", nominalData);
        fetch('/nominalthread/savenominalthread', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(nominalData),
        })
            .then(response => response.json())
            .then(data => {
                setnominalThreadData([...nominalThreadData, data]);
                getNominalThreads("");
                swal(`NominalThread has been ${nominalData.Id ? 'updated' : 'created'}.`);
            })
            .catch((error) => {
                console.error('Error:', error);
            });

    }


    //will send Editing Id to Add NominalThread component as props
    const sendEditingData = (EditingId) => {
        setNominalThreadId(EditingId);
        setIsModalOpen(true);
    }

    // This function is triggered from Search component for searching Queries
    const handleSearch = (searchParams) => {
        if (searchParams && searchParams !== null) {
            getNominalThreads(searchParams);
        }
        else {
            getNominalThreads('');
        }
    }

    //Function to handle delete operation for selected data
    const deleteNominalThread = async (Id) => {
        event.preventDefault();
        const data = { Id: Id };
        fetch('/nominalthread/delete', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        })
            .then(response => response.json())
            .then(data => {
                swal(data.message);
                getNominalThreads("");
            })
            .catch((error) => {
                console.error('Error:', error);
            });
    }
    // <> : React fragment to wrap multiple elements inside return statement.
    return (
        <>
            <div>
                <div className="pull-left">
                    <SearchNominalThread handleSearch={handleSearch} />
                </div>
                <div className="pull-right">
                    <button type="button" className="btn btn-primary" data-toggle="modal" data-target="#AddEditModal"
                        onClick={() => { setIsModalOpen(true) }}>Add a Nominal Thread</button>
                </div>
            </div>

            <table className="table caption-top table-bordered table-hover" >
                <thead>
                    <tr>
                        <th scope="col">Dash Number</th>
                        <th scope="col">Nominal ThreadSize</th>
                        <th scope="col">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        nominalThreadData.map((list, i) =>
                            <tr key={i}>
                                <td>{list.DashNumber}</td>
                                <td>{list.NominalThreadSize}</td>
                                <td>
                                    <div className="btn-toolbar">
                                        <button type="button" className="btn btn-primary" data-toggle="modal" data-target="#AddEditModal"
                                            onClick={() => sendEditingData(list.Id)}>Edit</button>
                                        <button type="button" className="btn btn-danger"
                                            onClick={() => deleteNominalThread(list.Id)}>Delete</button>
                                    </div>
                                </td>
                            </tr>
                        )
                    }

                </tbody>
            </table>

            {IsModalOpen == true ? <AddNominalThread onSubmit={addNewNominalThread} id={nominalThreadId}
                closeModal={() => { closeModal }} /> : null}

         
        </>
    )
}

//React render statement to render the React_Landing_Component to corresponding razor file with html element of id : root
ReactDOM.render(<NominalThreadTableView />, document.getElementById('root'));
