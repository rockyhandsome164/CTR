const Tests = (props) => {
    console.log('TEst_props', props);
    // is for Modal control
    const [IsModalOpen, setIsModalOpen] = React.useState(false);
    // Headers testTemplateHeaders as per required for Table.
    const testTableHeaders = ['Test', 'Name', 'Frequency', 'UnitOfMeasure', 'CyclesSamples',
        'ResultCycles', 'SeatLoad', 'Action'];


    //TestTemplate: to store the data from database for get function
    const [testTemplateData, setTestTemplateData] = React.useState([]);

    const fetchTestTemplate = async (searchParams) => {
        const url = `/itemspec/testtemplate?searchParams=${searchParams}`;
        try {
            const res = await fetch(url);
            const data = await res.json();
            console.log('testTempateData', data);
            await setTestTemplateData(data);
        } catch (err) {
            console.log(err);
        }
    };

    React.useEffect(() => {
        fetchTestTemplate('');
    }, []);


    const handleTestTemplateSearch = (searchParams) => {
        console.log("handleTestTemplateSearch in Test component", searchParams);
        if (searchParams && searchParams !== null) {
            fetchTestTemplate(searchParams);
        }
        else {
            fetchTestTemplate('');
        }

    }

    const handleTestSubmit = async (newPost) => {
        console.log('handleTestSubmit', newPost);
        const url = '/itemspec/savetests';
        await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(newPost),
        });
        /* await setDataToPost(newPost);*/
        swal(`Item has been updated`);
    };




    return (
        <>
            <div className="container-fluid">
                <div className="row" style={{ marginTop: '5px' }}>
                    <div className="btn-toolbar">
                        <button type="button" className="btn btn-info" data-toggle="modal" data-target="#AddTest"
                            onClick={() => { setIsModalOpen(true) }}>Add</button>

                        <button type="button" className="btn btn-primary" data-toggle="modal" data-target="#UpdateTest"
                            onClick={() => { setIsModalOpen(true) }}>Update</button>
                    </div>
                </div>

                <div className="row" style={{ marginTop: '5px' }}>

                </div>
            </div>

            {IsModalOpen == true ?
                <SharedModal
                    modalId={'UpdateTest'}
                    modalSize={'modal-xl'}
                    modalWidth={'50%'}
                    title={'Add Tests to Items'}
                    modalBody={
                        <TestTemplateForm />}

                /> : null}

            {IsModalOpen == true ?
                <SharedModal
                    modalId={'AddTest'}
                    modalSize={'modal-xl'}
                    modalWidth={'50%'}
                    header={'Search Items'}
                    title={'Test Template Data'}
                    modalBody={
                        <TestTemplateForm
                            testTemplateData={testTemplateData}
                            handleTestTemplateSearch={handleTestTemplateSearch}
                            handleTestSubmit={handleTestSubmit}
                            itemNumber={props.itemNumber}
                        />}


                /> : null}

        </>

    )
}