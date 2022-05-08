
const SharedModal = (props) => {
    const modalWidth = {
        width: `${props.modalWidth ? props.modalWidth : '80%'}`
    }
    
    //const RequiredProps = {
    //    header, 
    //title,
    //    modalBody,
    //    modalSize = size as per bootstrap3.4.1 . eg: modal-lg or nth
    //    onSubmit == need to submit the data to parent for submission of data
    //};   
    
    return (
        <>           
            <div className="modal fade" id={props.modalId} role="dialog" tabIndex="-1" aria-hidden="true" >
                <div className={`modal-dialog modal-dialog-centered ${props.modalSize ? props.modalSize : ""}`} style={modalWidth} role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                            <h4 className="modal-title" >{props.title}</h4>
                        </div>
                        <div className="modal-body">
                            {props.modalBody}
                        </div>                       
                    </div>
                </div>
            </div>
        </>

    )
}

