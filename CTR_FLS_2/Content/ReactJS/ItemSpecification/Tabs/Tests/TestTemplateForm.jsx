const TestTemplateForm = (props) => {
    console.log('TestTemplateProps', props);
    // is for Modal control
    const [IsModalOpen, setIsModalOpen] = React.useState(false);

    //Item onFields: to store the data from database for Post function
    const [dataToPost, setDataToPost] = React.useState([]);

    const handleTemplateSearch = (searchData) => {
        console.log("handleTemplateSearch in Testtemplate", searchData);
        props.handleTestTemplateSearch(searchData);

    }

    // will handle the changes of form input paramters.
    const handleInputChange = (event) => {
        const { name, value } = event.target;
        setDataToPost({ ...dataToPost, [name]: value });
    }

    // Headers testTemplateHeaders as per required for Table.
    const testTemplateHeaders = ['Test', 'Name', 'Frequency', 'UnitOfMeasure', 'CyclesSamples',
        'ResultCycles', 'Requirements', 'Select'];

    //Handles the  Select action on selecting table row from TestTemplateRow
    const SelectTestTeamplateTableRow = (selectedRow) => {
        console.log('selectedRow', selectedRow);
        $('#TemplateSearch').modal('hide');
        setDataToPost(selectedRow);
        props.handleTestTemplateSearch('');


    }
    const handleTestSubmit = (event) => {
        event.preventDefault();
        const url = '/itemspec/savetests';
        sendPostRequest(dataToPost, url);
    }

    const sendPostRequest = async (newPost, url) => {

        // todo : need to provide Job parameter
        var itemTestModel = { ...newPost, Job: 'Job', Item: props.itemNumber };

        console.log('handleTestSubmit', itemTestModel);
        await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(itemTestModel),
        });
        await setDataToPost(itemTestModel);
        /*swal(`Test has been updated`);*/
    };


    return (
        <>
            <form className="form-horizontal" onSubmit={handleTestSubmit} >
                <div className="panel panel-default">
                    <div className="container-fluid "  >
                        <div className="panel-heading">Test Definitions</div>
                        <div className="form-group col-sm-12">
                            <label htmlFor="Template" className="col-sm-3 control-label">Template:</label>
                            <div className="col-sm-2" style={{ display: 'inline-flex' }}>
                                <input type="text"
                                    className="form-control"
                                    id="Template"


                                />
                                <button
                                    type="button"
                                    className="input-group-addon"
                                    data-toggle="modal"
                                    data-target="#TemplateSearch"
                                    onClick={() => { setIsModalOpen(true) }}>
                                    <i className="glyphicon glyphicon-search"></i>
                                </button>
                            </div>

                            <label htmlFor="Type" className="col-sm-2 control-label">Type:</label>
                            <div className="col-sm-3">
                                <select
                                    disabled
                                    className="form-control"
                                    id='Type'
                                >
                                    <option value="">Select an Option</option>

                                </select>

                            </div>

                        </div>

                        <div className="form-group col-sm-12">
                            <label htmlFor="PSpec" className="col-sm-3 control-label">Performance Specification:</label>
                            <div className="col-sm-9" style={{ display: 'inline-flex' }}>
                                <input type="text"
                                    className="form-control"
                                    style={{ maxWidth: '440px' }}
                                    id="PSpec"

                                />
                                <button
                                    type="button"
                                    className="input-group-addon"
                                    data-toggle="modal"
                                    data-target="#PSpecSearch"
                                    onClick={() => { setIsModalOpen(true) }}>
                                    <i className="glyphicon glyphicon-search"></i>
                                </button>
                            </div>
                        </div>

                    </div>
                </div>

                <div className="panel panel-default">
                    <div className="container-fluid "  >
                        <div className="panel-heading">Parameters</div>
                        <div className="form-group col-sm-11">
                            <label htmlFor="Minimum" className="col-sm-7 control-label">No.of Decimals</label>
                        </div>
                        <div className="form-group col-sm-12">
                            <label htmlFor="Minimum" className="col-sm-3 control-label">Minimum:</label>
                            <div className="col-sm-2">
                                <input
                                    type="text"
                                    className="form-control"
                                    id="Minimum"

                                />
                            </div>

                            <div className="col-sm-1">
                                <input
                                    type="number"
                                    className="form-control"
                                />
                            </div>

                            <label htmlFor="UM" className="col-sm-2 control-label">U/M:</label>
                            <div className="col-sm-2" style={{ display: 'inline-flex' }}>
                                <input type="text"
                                    className="form-control"
                                    id="UM"
                                    name="UnitOfMeasure"
                                    value={dataToPost.UnitOfMeasure || ''}
                                    onChange={handleInputChange}

                                />
                                <button
                                    type="button"
                                    className="input-group-addon"
                                    data-toggle="modal"
                                    data-target="#UMSearch"
                                    onClick={() => { setIsModalOpen(true) }}>
                                    <i className="glyphicon glyphicon-search"></i>
                                </button>
                            </div>





                        </div>

                        <div className="form-group col-sm-12">
                            <label htmlFor="Maximum" className="col-sm-3 control-label">Maximum:</label>
                            <div className="col-sm-2">
                                <input
                                    type="text"
                                    className="form-control"
                                    id="Maximum"
                                />
                            </div>

                            <div className="col-sm-1">
                                <input
                                    type="number"
                                    className="form-control"
                                />
                            </div>
                        </div>

                        <div className="form-group col-sm-12">
                            <label htmlFor="SeatLoad" className="col-sm-3 control-label">Seat Load:</label>
                            <div className="col-sm-2">
                                <input
                                    type="text"
                                    className="form-control"
                                    id="SeatLoad"
                                    name="SeatLoad"
                                    value={dataToPost.SeatLoad || ''}
                                    onChange={handleInputChange}

                                />
                            </div>
                        </div>

                        <div className="form-group col-sm-12">
                            <label htmlFor="Requirement" className="col-sm-3 control-label">Requirements:</label>
                            <div className="col-sm-6">
                                <input
                                    type="text"
                                    className="form-control"
                                    id="Requirement"
                                    name="Requirements"
                                    value={dataToPost.Requirements || ''}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </div>



                    </div>

                </div>

                <div className="panel panel-default">
                    <div className="container-fluid "  >
                        <div className="panel-heading">Reporting Requirements</div>

                        <div className="form-group col-sm-12">
                            <label htmlFor="Cycle_Sample" className="col-sm-3 control-label">Cycles/Sample:</label>
                            <div className="col-sm-2">
                                <input
                                    type="text"
                                    className="form-control"
                                    id="Cycle_Sample"
                                    name="CyclesSamples"
                                    value={dataToPost.CyclesSamples || ''}
                                    onChange={handleInputChange}
                                />
                            </div>

                            <label htmlFor="Result_Cycle" className="col-sm-2 control-label">Result Cycles:</label>
                            <div className="col-sm-3">
                                <input
                                    type="text"
                                    className="form-control"
                                    id="Result_Cycle"
                                    name="ResultCycles"
                                    value={dataToPost.ResultCycles || ''}
                                    onChange={handleInputChange}
                                />
                            </div>
                            {/*<div className="col-sm-2">*/}
                            {/*    <button className="btn btn-default" >*/}
                            {/*        <label>Report Cycles</label>*/}
                            {/*    </button>*/}
                            {/*</div>*/}

                        </div>

                        <div className="form-group col-sm-12">
                            <label htmlFor="Frequency" className="col-sm-3 control-label"> Frequency:</label>
                            <div className="col-sm-2" >
                                <input type="text"
                                    className="form-control"
                                    id="Frequency"
                                    name="Frequency"
                                    value={dataToPost.Frequency || ''}
                                    onChange={handleInputChange}

                                />

                            </div>

                            <div className="col-sm-2">
                                <button className="btn btn-default" >
                                    <label>Report Cycles</label>
                                </button>
                            </div>
                        </div>

                    </div>
                </div>

                <div className="panel panel-default">
                    <div className="container-fluid "  >
                        <div className="panel-heading"> Test Bolts</div>


                        <div className="form-group col-sm-12">
                            <label htmlFor="TestBolt" className="col-sm-3 control-label">Test Bolt:</label>
                            <div className="col-sm-9" style={{ display: 'inline-flex' }}>
                                <input type="text"
                                    className="form-control"
                                    style={{ maxWidth: '245px' }}
                                    id="TestBolt"

                                />
                                <button
                                    type="button"
                                    className="input-group-addon"
                                    data-toggle="modal"
                                    data-target="#TestBoltcSearch"
                                    onClick={() => { setIsModalOpen(true) }}>
                                    <i className="glyphicon glyphicon-search"></i>
                                </button>
                            </div>
                        </div>

                        <div className="form-group col-sm-12">
                            <label htmlFor="BSpec" className="col-sm-3 control-label">Bolt Specification:</label>
                            <div className="col-sm-9" style={{ display: 'inline-flex' }}>
                                <input type="text"
                                    className="form-control"
                                    style={{ maxWidth: '245px' }}
                                    id="BSpec"

                                />
                                <button
                                    type="button"
                                    className="input-group-addon"
                                    data-toggle="modal"
                                    data-target="#BSpecSearch"
                                    onClick={() => { setIsModalOpen(true) }}>
                                    <i className="glyphicon glyphicon-search"></i>
                                </button>
                            </div>
                        </div>

                    </div>
                </div>


                <div className="container-fluid " style={{ display: 'flex', justifyContent: 'center' }} >

                    <button className="btn btn-info" type="submit">OK</button>
                    <button className="btn btn-success" style={{ marginLeft: '10px' }} type="button">Done</button>
                    <button className="btn btn-danger" style={{ marginLeft: '10px' }} type="button" >Cancel</button>

                </div>


            </form>

            {IsModalOpen == true ?
                <SharedModal
                    modalId={'TemplateSearch'}
                    modalSize={'modal-xl'}
                    header={'Search Items'}
                    title={'Test Template Data'}
                    modalBody={
                        <SharedForm
                            label={"Search Items"}
                            formColSize={'12'}
                            handleSearch={handleTemplateSearch}
                            id={"searchItems"}
                            placeholder={'Test Templates...'}
                            searchTable={
                                <SharedTable
                                    tableColSize={'12'}
                                    headers={testTemplateHeaders}
                                    rows={props.testTemplateData}
                                    selectTableRow={SelectTestTeamplateTableRow}
                                />
                            }

                        />}

                /> : null}

        </>
    )
}