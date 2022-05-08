const ModalFooter = (props) => {

    return (
        <>
            <div className="modal-footer">
                <button type="button" className="btn btn-default" data-dismiss="modal">Close</button>
                <button type="button" className="btn btn-success"
                    onClick={() => props.searchButton()}>
                    {props.buttonlabel || 'Search'}</button>
            </div>

        </>)
}