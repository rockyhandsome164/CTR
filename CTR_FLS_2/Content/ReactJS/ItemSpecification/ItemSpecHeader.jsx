
const ItemSpecHeader = (props) => {
    // is for Modal control
    const [IsModalOpen, setIsModalOpen] = React.useState(false);

    //Item onFields: to store the data from database for get function
    const [dataToPost, setDataToPost] = React.useState({ Status: 'I' });

    //will call the fxn: fetchItems when component is rendered.
    React.useEffect(() => {
        fetchItems('');
        getNominalThreads('');
    }, []);

    //Headers Items as per required for Table.
    const NominalThreadheaders = ['DashNumber', 'NominalThreadSize', 'Action'];

    //nominalThreadData: to store the data from database for get function
    const [nominalThreadData, setNominalThreadData] = React.useState([]);

    //get NominalThread Data 
    const getNominalThreads = async (searchParams) => {
        const url = `/nominalthread/getallnominalthreads?searchParams=${searchParams}`;
        const res = await axios.get(url);
        const { data } = await res;
        setNominalThreadData(data);
    }

    // will handle the handlesearch function passed to Search component & searchData received from searchComponent
    const handleNominalSearch = (searchParams) => {
        if (searchParams && searchParams !== null) {
            getNominalThreads(searchParams);
        }
        else {
            getNominalThreads('');
        }
    }

    //Handles the  Select action on selecting table row from NominalThreadTable
    const selectNominalTableRow = (selectedRow) => {
        setDataToPost({ ...dataToPost, NominalThreadSize: selectedRow.NominalThreadSize });
    }

    //Headers Items as per required for ProductFamily Table.
    const ProductFamilyHeaders = ['ProductFamily', 'Description', 'Action'];

    //ProductFamilyData: to store the data from database for get function
    const [productFamilyData, setProductFamilyData] = React.useState([]);

    const fetchProductFamily = async (searchParams) => {
        const url = `/itemspec/getproductfamily?searchParams=${searchParams}`;
        try {
            const res = await fetch(url);
            const data = await res.json();
            await setProductFamilyData(data);

        } catch (err) {
            console.log(err);
        }
    };

    React.useEffect(() => {
        fetchProductFamily('');
    }, []);

    const handleProductFamilySearch = (searchParams) => {
        if (searchParams && searchParams !== null) {
            fetchProductFamily(searchParams);
        }
        else {
            fetchProductFamily('');
        }
    }

    //Handles the  Select action on selecting table row from NominalThreadTable
    const selectProductFamilyTableRow = (selectedRow) => {
        setDataToPost({ ...dataToPost, ProductFamily: selectedRow.ProductFamily });
    }

    //Headers Items as per required for Table. 
    const Itemheaders = ['Item1', 'Description', 'NominalThreadSize', 'MaterialStatus', 'Type', 'ProductFamily', 'Status', 'Action'];

    //Items: to store the data from database for get function
    const [items, setItems] = React.useState([]);

    //get Items Data 
    const fetchItems = async (searchParams) => {
        let url = `/itemspec/items?searchParams=${searchParams}`;
        const res = await axios.get(url);
        const { data } = await res;
        setItems(data);
    }

    // will handle the handlesearch function passed to Search component & searchData received from searchComponent
    const handleItemSearch = (searchParams) => {
        if (searchParams && searchParams !== null) {
            fetchItems(searchParams);
        } else {
            fetchItems('');
        }
    }

    //Handles the  Select action on selecting table row from ItemsTable
    const SelectItemTableRow = (selectedRow) => {
        props.setItemNumber(selectedRow.Item1);
        setDataToPost(selectedRow);
        fetchItems('');
    }

    // will handle the changes of form input paramters.
    const handleInputChange = (event) => {
        const { name, value } = event.target;
        setDataToPost({ ...dataToPost, [name]: value });
    }

    const sendPostRequest = async (newPost, url) => {
        await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(newPost),
        });
        await setDataToPost(newPost);
        swal(`Item has been updated`);
    };

    //Handles the Post Method of items form
    const handleSubmit = (event) => {
        let url = '/itemspec/saveitems';
        event.preventDefault();
        sendPostRequest(dataToPost, url);
    }

    return (
        <>
            <nav className="">
                <div className="container-fluid">
                    <div className="row" style={{ marginTop: '25px' }}>
                        <div className="col-md-10">

                            <form className="form-horizontal" onSubmit={handleSubmit}>
                                <div className="col-md-5">

                                    <div className="form-group">
                                        <label htmlFor="item1" className="col-sm-4 control-label">Item:</label>
                                        <div className="col-sm-7" style={{ display: 'inline-flex', inlineSize: '310px' }}>
                                            <input
                                                type="text"
                                                className="form-control"
                                                id="item1"
                                                name="Item1"
                                                value={dataToPost.Item1 || ''}
                                                onChange={handleInputChange}
                                            />
                                            <button
                                                type="button"
                                                className="input-group-addon"
                                                data-toggle="modal"
                                                data-target="#Items"
                                                onClick={() => { setIsModalOpen(true) }}>
                                                <i className="glyphicon glyphicon-search"></i>
                                            </button>
                                        </div>
                                    </div>

                                    <div className="form-group">
                                        <label htmlFor="description" className="col-sm-4 control-label">Description:</label>
                                        <div className="col-sm-7">
                                            <input
                                                type="text"
                                                className="form-control"
                                                id="description"
                                                name="Description"
                                                value={dataToPost.Description || ''}
                                                onChange={handleInputChange}
                                            />
                                        </div>
                                    </div>

                                    <div className="form-group">
                                        <label htmlFor="genericdesc" className="col-sm-4 control-label">Generic Desc:</label>
                                        <div className="col-sm-7">
                                            <input
                                                type="text"
                                                className="form-control"
                                                id="genericdesc"
                                                name="Type"
                                                value={dataToPost.Type || ''}
                                                onChange={handleInputChange}
                                            />
                                        </div>
                                    </div>

                                </div>

                                <div className="col-md-7">
                                    <div className="form-group">
                                        <label htmlFor="nominalThreadSize" className="col-sm-4 control-label">Nominal ThreadSize:</label>

                                        <div className="col-sm-5" style={{ display: 'inline-flex', inlineSize: '310px' }}>
                                            <input
                                                type="text"
                                                className="form-control"
                                                id="nominalThreadSize"
                                                name="NominalThreadSize"
                                                value={dataToPost.NominalThreadSize || ''}
                                                onChange={handleInputChange}
                                            />
                                            <button
                                                type="button"
                                                className="input-group-addon"
                                                data-toggle="modal"
                                                data-target="#NominalThreadSize"
                                                onClick={() => { setIsModalOpen(true) }}>
                                                <i className="glyphicon glyphicon-search"></i>
                                            </button>
                                        </div>

                                    </div>

                                    <div className="form-group">
                                        <label htmlFor="productFamily" className="col-sm-4 control-label">Product Family:</label>

                                        <div className="col-sm-5" style={{ display: 'inline-flex', inlineSize: '310px' }}>
                                            <input
                                                type="text"
                                                className="form-control"
                                                id="productFamily"
                                                name="ProductFamily"
                                                value={dataToPost.ProductFamily || ''}
                                                onChange={handleInputChange}
                                            />
                                            <button
                                                type="button"
                                                className="input-group-addon"
                                                data-toggle="modal"
                                                data-target="#ProductFamily"
                                                onClick={() => { setIsModalOpen(true) }}>
                                                <i className="glyphicon glyphicon-search"></i>
                                            </button>
                                        </div>
                                    </div>

                                    <div className="form-group">
                                        <label htmlFor="ItemStatus" className="col-sm-4 control-label">Item Status:</label>
                                        <div className="col-sm-8" >
                                            <label className="radio-inline">
                                                <input
                                                    id="i"
                                                    type="radio"
                                                    value="I"
                                                    name="Status"
                                                    onChange={handleInputChange}
                                                    checked={dataToPost.Status === "I"}
                                                /> InComplete
                                            </label>
                                            <label className="radio-inline">
                                                <input
                                                    id="c"
                                                    type="radio"
                                                    value="C"
                                                    name="Status"
                                                    checked={dataToPost.Status === "C"}
                                                    onChange={handleInputChange}
                                                /> Complete
                                            </label>
                                            <label className="radio-inline">

                                                <input
                                                    id="n"
                                                    type="radio"
                                                    value="N"
                                                    name="Status"
                                                    checked={dataToPost.Status === "N"}
                                                    onChange={handleInputChange}

                                                />Not Applicable
                                            </label>
                                        </div>
                                    </div>

                                </div>

                                <div className="col-sm-11 " style={{ marginTop: "20px", marginBottom: "10px" }} align="center" >
                                    <div style={{ display: "inline-flex" }}>
                                        <input id="SearchButton" className="btn btn-success" type="submit" value="Submit" />
                                        <button
                                            id="ResetButton"
                                            className="btn btn-info"
                                            type="button"
                                            onClick={() => setDataToPost({ Status: 'I' })}
                                            style={{ marginLeft: "40px" }}
                                        >
                                            Reset
                                        </button>
                                    </div>
                                </div>
                            </form>

                        </div>

                        {
                            dataToPost && dataToPost.Item1
                                ? <div className="col-md-2">
                                    <NoteView itemNumber={dataToPost.Item1 || null} />
                                </div>
                                : null

                        }


                    </div>
                </div>
            </nav>

            {IsModalOpen == true ?
                <SharedModal
                    modalId={'Items'}
                    modalSize={'modal-xl'}
                    header={'Search Items'}
                    title={'Searching'}
                    modalBody={
                        <SharedForm
                            label={"Search Items"}
                            formColSize={'8'}
                            tableColSize={'9'}
                            handleSearch={handleItemSearch}
                            id={"searchItems"}
                            placeholder={'Item Number or Description...'}
                            searchTable={
                                <SharedTable
                                    headers={Itemheaders}
                                    rows={items}
                                    selectTableRow={SelectItemTableRow}
                                />
                            }
                        />}

                /> : null}

            {IsModalOpen == true ?
                <SharedModal
                    modalId={'NominalThreadSize'}
                    modalSize={'modal-xl'}
                    header={'Search Items'}
                    title={'NominalThreadSize Data'}
                    modalBody={
                        <SharedForm
                            label={"Search Items"}
                            formColSize={'6'}
                            tableColSize={'9'}
                            handleSearch={handleNominalSearch}
                            id={"searchItems"}
                            placeholder={'Nominal ThreadSize...'}
                            searchTable={
                                <SharedTable
                                    headers={NominalThreadheaders}
                                    rows={nominalThreadData}
                                    selectTableRow={selectNominalTableRow}
                                />
                            }
                        />}

                /> : null}

            {
                IsModalOpen == true ?
                    <SharedModal
                        modalId={'ProductFamily'}
                        modalSize={'modal-xl'}
                        header={'Search Product'}
                        title={'ProductFamily Data'}
                        modalBody={
                            <SharedForm
                                label={"Search Items"}
                                formColSize={'6'}
                                tableColSize={'9'}
                                handleSearch={handleProductFamilySearch}
                                id={"searchPoduct"}
                                placeholder={'ProductFamily...'}
                                searchTable={
                                    <SharedTable
                                        headers={ProductFamilyHeaders}
                                        rows={productFamilyData}
                                        selectTableRow={selectProductFamilyTableRow}
                                    />
                                }
                            />}

                    /> : null
            }

        </>
    )
}