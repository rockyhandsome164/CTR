const ItemSpecLandingPage = () => {
    const [itemNumber, setItemNumber] = React.useState();

    React.useEffect(() => {       
    }, [itemNumber]);

    return (
        <>
            <div>
                <ItemSpecHeader setItemNumber={setItemNumber} />
       
                <div>                  
                    <ul className="nav nav-tabs" role="tablist">
                        <li role="presentation" className=" nav-link active"><a href="#specification" aria-controls="specification" role="tab" data-toggle="tab">Specification</a></li>
                        <li role="presentation"><a href="#tests" className="nav-link" aria-controls="tests" role="tab" data-toggle="tab">Tests</a></li>
                        {/*<li role="presentation"><a href="#bolts" className="nav-link" aria-controls="bolts" role="tab" data-toggle="tab">Bolts</a></li>*/}
                        
                    </ul>

                    <div className="tab-content">
                        <div role="tabpanel" className="tab-pane active" id="specification"> <Specification itemNumber={itemNumber || null} /> </div>
                        <div role="tabpanel" className="tab-pane" id="tests"> <Tests itemNumber={itemNumber || null} /> </div>
                        <div role="tabpanel" className="tab-pane" id="bolts"><Bolts/> </div>
                        
                    </div>

                </div>
              
            </div>
        </>)
}


//React render statement to render the React_Landing_Component to corresponding razor file with html element of id : root
ReactDOM.render(<ItemSpecLandingPage />, document.getElementById('root'));