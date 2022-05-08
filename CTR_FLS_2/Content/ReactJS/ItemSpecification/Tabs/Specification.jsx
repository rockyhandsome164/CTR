const Specification = (props) => {

    //ItemSpecs: to store the data from database for get function
    const [itemSpecs, setItemSpecs] = React.useState([]);
    const [masterSpec, setMasterSpec] = React.useState([]);
    const [defaultType, setDefaultType] = React.useState([]);

    // is for Modal control
    const [IsModalOpen, setIsModalOpen] = React.useState(false);
    const ItemSpecTableHeaders = ['Item', 'ItemType', 'Specification', 'Description', 'DateEntered', 'EnteredBy'];

    const handleMasterSpecSearch = (searchParams) => {
        if (searchParams && searchParams !== null) {
            getMasterSpecs(searchParams);
        } else {
            getMasterSpecs('');
        }
    }

    const fetchItemSpecByitemNumber = async (itemNumber) => {
        let url = `/itemspec/getitemspecsbyitemnumber?itemNumber=${itemNumber}`;
        const res = await fetch(url);
        const data = await res.json();

        try {

            const newData = data.map(item => {
                const DisplayDateEntered = new Date(item.DateEntered.match(/\d+/)[0] * 1).toLocaleDateString();
                item.DateEntered = DisplayDateEntered;
                const { Spec: Specification, ...rest } = item;
                return { Specification, ...rest }
            }
            );
            await setItemSpecs(newData);
        } catch (err) {
            console.log(err);
        }
    };

    getMasterSpecs = async (searchParams) => {
        let paramsObj = new URLSearchParams(searchParams).toString();
        let url = "/utilities/search?" + paramsObj;
        const response = await fetch(url);
        const data = await response.json();
        const masterspecData = [];

        //need this fxn to map inner Array DefaultType element
        data.forEach((element) => {
            let info = {
                Id: element.Id,
                Specification: element.Specification ? element.Specification : null,
                Description: element.Description ? element.Description : null,
                Master: element.Master ? element.Master : null,
                DefaultType: element.DefaultType.DefaultType1 ? element.DefaultType.DefaultType1 : null,
                EnteredBy: element.EnteredBy,
                DateEntered: new Date(element.DateEntered.match(/\d+/)[0] * 1).toLocaleDateString(),
                DefaultTypeId: element.DefaultTypeId
            };
            masterspecData.push(info);
        });
        setMasterSpec(masterspecData);
    }

    //to get ItemNumber from ItemSpecHeader Component when select Item is clicked
    React.useEffect(() => {

        if (props.itemNumber) {
            fetchItemSpecByitemNumber(props.itemNumber);
        }
    }, [props.itemNumber]);


    //get spec_DefaultType  Data 
    const getMasterDefaultType = async () => {
        const url = '/utilities/gettypes';
        const res = await axios.get(url);
        const { data } = await res;
        setDefaultType(data);
    }

    const afterCloseModal = () => {
        console.log('Closed');
        //function to refresh
    }

    const addMasterSpecsToItemSpecs = async (data) => {
        console.log("masterSpecsData", data);

        var itemSpecModel = {
            ItemNumber: props.itemNumber,
            Spec: data.Specification,
            ItemType: data.DefaultType,
            Description: data.Description
        }
        console.log('itemSpecModel', itemSpecModel);

        let url = `/itemspec/saveitemspec`;

        await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(itemSpecModel),
        });

        await fetchItemSpecByitemNumber(props.itemNumber);

    }
    // will render the DefaultType and masterSpec Data when page is rendered
    React.useEffect(() => {
        getMasterDefaultType();
        getMasterSpecs();
    }, []);

    return (
        <>
            <div className="container-fluid">
                <div className="row" style={{ marginTop: '5px' }}>
                    <div className="btn-toolbar">
                        {props.itemNumber
                            ? <button type="button" className="btn btn-primary" data-toggle="modal" data-target="#AddEditModal"
                                onClick={() => { setIsModalOpen(true) }}>New</button>
                            : null}
                        {/*<button type="button" className="btn btn-primary" data-toggle="modal" data-target="#CreateMasterSpec"*/}
                        {/*    onClick={() => { setIsModalOpen(true) }}>Create</button>*/}
                        {/*<button type="button" className="btn btn-info">Audit</button>*/}
                    </div>
                </div>

                <div className="row" style={{ marginTop: '5px' }}>
                    <SharedTable
                        headers={ItemSpecTableHeaders}
                        rows={itemSpecs || null}
                    />
                </div>
            </div>
            {IsModalOpen == true ?
                <SearchForm
                    masterSpec={masterSpec}
                    defaultType={defaultType}
                    handleSearch={handleMasterSpecSearch}
                    handleSelectMasterSpecTableRow={addMasterSpecsToItemSpecs}
                    afterCloseModal={afterCloseModal}
                />
                : null}

        </>
    )
}