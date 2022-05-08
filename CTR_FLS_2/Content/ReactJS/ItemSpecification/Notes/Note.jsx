const Note = () => {

    //CSS for NoteView
    const note = {
        backgroundColor: '#fef68a',
        borderRadius: '10px',
        padding: '1rem',
        minHeight: '170px',
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'space-between',
        whiteSpace: 'pre-wrap'
    }
    const noteFooter = {
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
    }

    const text = 'Note';
    const date = new Date();

    return (
        <>

            <SharedForm
                label={"Enter Notes"}
                value={noteData.Notes}
                formColSize={'12'}
                tableColSize={'4'}
                Type={'textarea'}
                handleSearch={upsertNote}
                id={"textArea"}
                buttonlabel={'Save Note'}
                searchTable={
                    null
                }
            />

 

        </>)
}


//<SharedForm
//    label={"Enter Notes"}
//    value={noteData.Notes}
//    formColSize={'8'}
//    tableColSize={'4'}
//    Type={'textarea'}
//    handleSearch={upsertNote}
//    id={"textArea"}
//    buttonlabel={'Save Note'}
//    searchTable={
//        null
//    }
///>