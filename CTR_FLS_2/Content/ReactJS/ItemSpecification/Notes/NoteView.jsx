
// this should get the notes of that item number as the props
// like props.note
// when we save the note we should be saving the note with the textKey 
//pass the textkey along with the note 
const NoteView = (props) => {
    const [IsModalOpen, setIsModalOpen] = React.useState(false);
    const [noteData, setNoteData] = React.useState([]);
    const [currentlyActiveNote, setCurrentlyActiveNote] = React.useState({});
    
     const fetchItemNoteByitemNumber = async (itemNumber) => {
        let url = `/itemspec/getnotebyitemnumber?itemNumber=${itemNumber}`;
        const res = await fetch(url);
        const itemNote = await res.json();

         try {
             console.log("allnotes", itemNote)
            setNoteData(itemNote)
            setCurrentlyActiveNote(itemNote[0]);

        } catch (err) {
            console.log(err);
        }
    };

    const Itemheaders = ['Notes', 'Select'];


    //to get ItemNumber from ItemSpecHeader Component when select Item is clicked
    React.useEffect(() => {

        if (props.itemNumber) {
            fetchItemNoteByitemNumber(props.itemNumber);
        }
    }, [props.itemNumber]);


    const upsertNote = async (notes) => {
        currentlyActiveNote.Notes = notes;
       await fetch('/itemspec/savenote', {
            method: 'POST',headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(currentlyActiveNote),
        })
       await fetchItemNoteByitemNumber(props.itemNumber);
       swal(`Note has been updated`);
    }

    //Handles the  Select action on selecting table row from ItemsTable
    const SelectItemTableRow = (selectedRow) => {
        setCurrentlyActiveNote(selectedRow);
    }

    return (
        <>
            <div style={{ display: "inline-flex" }}>
                <button
                    type="button"
                    className="btn btn-success"
                    data-toggle="modal"
                    data-target="#NoteView"
                    onClick={() => { setIsModalOpen(true) }}>
                    <span className="glyphicon glyphicon-edit" aria-hidden="true"></span> Note
                </button>
            </div>

            {
                IsModalOpen == true ?
                    <SharedModal
                        modalId={'NoteView'}
                        modalSize={'modal-lg'}
                        header={'Search Items'}
                        title={'NoteView'}
                        modalWidth={'50%'}
                        modalBody={
                            <SharedForm
                                label={"Enter Notes"}
                                value={currentlyActiveNote.Notes}
                                formColSize={'12'}
                                Type={'textarea'}
                                handleSearch={upsertNote}
                                id={"textArea"}
                                buttonlabel={'Save'}
                                searchTable={
                                    <SharedTable
                                        headers={Itemheaders}
                                        rows={noteData}
                                        selectTableRow={SelectItemTableRow}
                                    />
                                }
                            />}
                    /> : null
            }

        </>)
}